
$(document).ready(function () {
    // Fetch the dashboard data from AdminFeaturesController
    $.ajax({
        url: '/AdminFeatures/DashBoardCount',
        type: 'GET',
        success: function (data) {
            if (data.error) {
                alert('Error' + data.error);
            } else {
                $('#activeSensors').text(data.ActiveSensors);
                $('#inActiveSensors').text(data.InActiveSensors);
                $('#totalUsers').text(data.TotalUsers);
                $('#adminAcc').text(data.AdminUsers);

                // Call the function to render the bar chart
                renderSensorBarChart(data.ActiveSensors, data.InActiveSensors);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data: " + error);
        }
    });

    // Function to stop data generation for sensors
    $('#stopDataGen').on('click', function () {
        // Check if all sensors are already inactive 
        $.ajax({
            url: '/Sensor/GetSensorStatus',
            type: 'GET',
            data: { checkForActive: false },  // Check for inactive status
            success: function (response) {
                if (response.allInactive) {
                    // Show SweetAlert if all sensors are already inactive
                    Swal.fire({
                        icon: 'info',
                        title: 'All Sensors Already Inactive',
                        text: 'The status of all sensors is already set to inactive.',
                    }).then((result) => {
                        // Reload the page after the user clicks OK on the SweetAlert
                        if (result.isConfirmed) {
                            window.location.reload();
                        }
                    });
                }
                else {
                    // Set all sensors to inactive 
                    $.ajax({
                        url: '/Sensor/SetSensorsStatusInactive',
                        type: 'POST',
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Sensors Set to Inactive',
                                    text: response.message,
                                }).then((result) => {
                                    // Reload the page after the user clicks OK on the SweetAlert
                                    if (result.isConfirmed) {
                                        window.location.reload();
                                    }
                                });
                            }
                            else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Error',
                                    text: response.message,
                                });
                            }
                        },
                        error: function () {
                            Swal.fire({
                                icon: 'error',
                                title: 'An error occurred while setting sensors to inactive.',
                            });
                        }
                    });
                }
            }
        });
    });

    // Function to start data generation for sensors
    $('#startDataGen').on('click', function () {
        $.ajax({
            url: '/Sensor/GetSensorStatus',
            type: 'GET',
            data: { checkForActive: true },  // Check for active status
            success: function (response) {
                if (response.allActive) {
                    // Show SweetAlert if all sensors are already active
                    Swal.fire({
                        icon: 'info',
                        title: 'All Sensors Already Active',
                        text: 'The status of all sensors is already set to active.',
                    }).then((result) => {
                        // Reload the page after the user clicks OK on the SweetAlert
                        if (result.isConfirmed) {
                            window.location.reload();
                        }
                    });
                }
                else {
                    // Set all sensors to active 
                    $.ajax({
                        url: '/Sensor/SetSensorsStatusActive',
                        type: 'POST',
                        success: function (response) {
                            if (response.success) {
                                Swal.fire({
                                    icon: 'success',
                                    title: 'Sensors Set to Active',
                                    text: response.message,
                                }).then((result) => {
                                    // Reload the page after the user clicks OK on the SweetAlert
                                    if (result.isConfirmed) {
                                        window.location.reload();
                                    }
                                });
                            }
                            else {
                                Swal.fire({
                                    icon: 'error',
                                    title: 'Error',
                                    text: response.message,
                                });
                            }
                        },
                        error: function () {
                            Swal.fire({
                                icon: 'error',
                                title: 'An error occurred while setting sensors to active.',
                            });
                        }
                    });
                }
            }
        });
    });
});

// Function to render the bar chart using Chart.js
function renderSensorBarChart(active, inactive) {
    // Create data for the bar chart
    var chartData = {
        // X-axis labels
        labels: ['Active Sensors', 'Inactive Sensors'],
        datasets: [{
            // Chart label
            label: 'Sensor Status',
            // Data for each bar
            data: [active, inactive],
            // Colors for the bars (green for active, red for inactive)
            backgroundColor: ['#4CAF50', '#FF5722']
        }]
    };

    // Create the bar chart
    var ctx = document.getElementById('sensorChart').getContext('2d');
    var myBarChart = new Chart(ctx, {
        type: 'bar',
        data: chartData,
        options: {
            scales: {
                y: {
                    beginAtZero: true
                }
            },
            responsive: true,
            plugins: {
                legend: {
                    position: 'top',
                }
            }
        }
    });
}

