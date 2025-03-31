$(document).ready(function () {
    // Load sensors when the page is ready 
    fetchSensors();

    // Handle search functionality 
    $('#searchSensorBtn').on('click', function () {
        fetchSensors();
    });

    // Get sensors based on the location search query 
    function fetchSensors() {
        var searchQuery = $('#searchSensor').val().trim();

        $.ajax({
            url: '/Sensor/GetSensors',
            type: 'GET',
            data: { searchQuery: searchQuery, timestamp: new Date().getTime() },
            success: function (data) {
                console.log("Sensors fetched after update:", data);
                // Verify if data is coming correctly
                console.log(data);
                // Clear the table
                $('#sensorTable tbody').empty();

                if (data.length > 0) {
                    data.forEach(function (sensor) {
                        // Log each sensor to ensure properties exist
                        console.log(sensor);

                        // Constructing the row with the correct object properties
                        var row = `<tr data-id="${sensor.Id}">
                        <td>${sensor.Name}</td>
                        <td>${sensor.Location}</td>
                        <td>${sensor.Status}</td>
                        <td>${sensor.Latitude}</td>
                        <td>${sensor.Longitude}</td>
                        <td>
                            <button class="editBtn">Edit</button>
                            <button class="deleteBtn">Delete</button>
                        </td>
                    </tr>`;
                        // Append the row to the table
                        $('#sensorTable tbody').append(row);
                    });
                } else {
                    $('#sensorTable tbody').append('<tr><td colspan="4">No sensor found</td></tr>');
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching sensors: " + error);
            }
        });
    }
    // Handle click on Edit button

    $(document).on('click', '.editBtn', function () {
        var sensorId = $(this).closest('tr').data('id');
        var sensorRow = $(this).closest('tr');
        var sensorName = sensorRow.find('td').eq(0).text();
        var sensorLocation = sensorRow.find('td').eq(1).text();
        var sensorStatus = sensorRow.find('td').eq(2).text();
        var sensorLatutude = sensorRow.find('td').eq(3).text();
        var sensorLongitude = sensorRow.find('td').eq(4).text();



        // Populate the form with the selected sensor's data
        $('#name').val(sensorName);
        $('#location').val(sensorLocation);
        $('#status').val(sensorStatus);
        $('#latitude').val(sensorLatutude);
        $('#longitude').val(sensorLongitude);


        // Change the form title to "Edit Sensor"
        $('#formTitle').text('Edit Sensor');
        $('#sensorForm').show(); // Show the form for editing

        // Attach the sensorId to the form for later use
        $('#sensorForm').data('sensorId', sensorId);

        // Show the "Back to Add New" button
        $('#backToAddNew').show();
    });

    // "Back to Add New" button logic
    $('#backToAddNew').on('click', function () {
        // Reset form to "Add New" state
        resetForm();
        // Hide the "Back to Add New" button
        $('#backToAddNew').hide();
    });


    // Add new sensor Button
    $('#addNewBtn').on('click', function () {
        resetForm();
        $('#formTitle').text('Add New Sensor');
        $('#sensorForm').show();
        // Hide the "Back to Add New" button
        $('#backToAddNew').hide();
    });

    //// Submit the form (either Add or Edit)
    $('#submitSensor').on('click', function () {
        var sensorData = {
            name: $('#name').val(),
            location: $('#location').val(),
            status: $('#status').val(),
            Latitude: parseFloat($('#latitude').val()),
            Longitude: parseFloat($('#longitude').val()),
            // Ensure the id is being passed
            id: $('#sensorForm').data('sensorId')
        };
        // Add this log to confirm
        console.log("Updating sensor with ID: " + sensorData.id);

        if ($('#formTitle').text() === 'Add New Sensor') {
            // Add sensor
            addNewSensor(sensorData);
        } else {
            // Update sensor
            updateSensor(sensorData);
        }
    });


    // Add a new sensor via AJAX

    function addNewSensor() {
        // Get form data
        var sensorData = {
            Name: $('#name').val(),
            Location: $('#location').val(),
            Status: $('#status').val(),
            Latitude: parseFloat($('#latitude').val()),
            Longitude: parseFloat($('#longitude').val())

        };

        // Check if latitude and longitude are numbers
        if (isNaN(sensorData.Latitude) || isNaN(sensorData.Longitude)) {
            Swal.fire('Error!', 'Latitude and Longitude must be valid numbers.', 'error');
            return;
        }

        // Send data via AJAX
        $.ajax({
            // URL to your controller method
            url: '/Sensor/AddSensor',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(sensorData),
            success: function (response) {
                if (response.success) {
                    Swal.fire('Success!', 'Sensor added successfully!', 'success');
                    // Reload the sensor list after adding a new sensor
                    fetchSensors();
                    // Reset the form fields after adding
                    resetForm();
                    // Hide the "Back to Add New" button
                    $('#backToAddNew').hide();
                } else {
                    Swal.fire('Error!', 'Failed to add sensor.', 'error');
                }
            },
            error: function (xhr, status, error) {
                Swal.fire('Error!', 'An error occurred: ' + error, 'error');
            }
        });
    }


    // Edit existing sensor data via AJAX
    function updateSensor(sensorData) {
        console.log("Updating sensor with ID: " + sensorData.id);
        console.log("Sensor Data:", sensorData);

        $.ajax({
            // Correct URL to controller method
            url: '/Sensor/UpdateSensor',
            type: 'POST',
            data: sensorData,
            success: function () {
                Swal.fire('Success!', 'Sensor has been updated.', 'success');
                // Reload the sensor list after adding a new sensor
                fetchSensors();
                // Reload the sensor list
                resetForm();
                // Hide the "Back to Add New" button
                $('#backToAddNew').hide();
            },
            error: function (xhr, status, error) {
                Swal.fire('Error!', 'Something went wrong. Please try again.', 'error');
            }
        });
    }

    // Delete sensor via AJAX
    $(document).on('click', '.deleteBtn', function () {
        let sensorId = $(this).closest('tr').data('id');

        Swal.fire({
            title: 'Are you sure?',
            text: 'You are about to delete this sensor! All the AQI Data Related To This Sensor Will Be Deleted AS Well!!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    // URL to  controller method
                    url: '/Sensor/DeleteSensor',
                    type: 'POST',
                    data: { id: sensorId },
                    success: function () {
                        Swal.fire('Deleted!', 'Sensor has been deleted.', 'success');
                        // Reload the sensor list after deletion
                        fetchSensors();
                    },
                    error: function (xhr, status, error) {
                        Swal.fire('Error!', 'Something went wrong. Please try again.', 'error');
                    }
                });
            }
        });
    });

    // Reset the form for adding or editing sensors
    function resetForm() {
        $('#name').val('');
        $('#location').val('');
        $('#status').val('Active');
        $('#latitude').val('');
        $('#longitude').val('');
        // Do not hide the form, just reset it
        // Hide the "Back to Add New" button
        $('#sensorForm').show();
        // Reset the form title to "Add New Sensor"
        $('#formTitle').text('Add New Sensor');
    }

});

