$(document).ready(function () {
    // Load sensors and map when the page is ready 
    fetchDataAndCreateMap();

    function fetchDataAndCreateMap() {
        $.ajax({
            url: '/DynamicData/GetAirQuality',
            type: 'GET',
            success: function (data) {
                if (data && data.length > 0) {
                    // Part 1: Update the Air Quality Table
                    var tableBody = $('#airQuality tbody');
                    tableBody.empty(); // Clear any existing rows

                    data.forEach(function (item) {
                        var dateFormatted = item.ReadingDate
                            ? new Date(parseInt(item.ReadingDate.replace('/Date(', '').replace(')/', ''))).toLocaleDateString()
                            : 'No data';

                        var row = `
                            <tr data-id="${item.SensorId}">
                                <td>${item.SensorId}</td>
                                <td>${item.SensorLocation || 'No location'}</td>
                                <td>${item.SensorStatus || 'No status'}</td>
                                <td>${item.SensorLatitude || 'N/A'}</td>
                                <td>${item.SensorLongitude || 'N/A'}</td>
                                <td>${item.PM2_5 !== null ? item.PM2_5 : 'N/A'}</td>
                                <td>${item.PM10 !== null ? item.PM10 : 'N/A'}</td>
                                <td>${item.Pm1 !== null ? item.Pm1 : 'N/A'}</td>
                                <td>${item.RH !== null ? item.RH : 'N/A'}</td>
                                <td>${item.Temp !== null ? item.Temp : 'N/A'}</td>
                                <td>${item.Wind !== null ? item.Wind : 'N/A'}</td>
                                <td>${item.Aqi !== null ? item.Aqi : 'N/A'}</td>
                                <td>${dateFormatted}</td>
                            </tr>
                        `;
                        tableBody.append(row); // Add rows to table body
                    });

                    // Part 2: Create the Map and Add Markers
                    createMapWithMarkers(data);

                } else {
                    $('#airQuality tbody').append('<tr><td colspan="13">No data available</td></tr>');
                }
            },

            error: function (error) {
                console.error("Error fetching data: ", error);
                alert("An error occurred while fetching data.");
            }
        });
    }

    function createMapWithMarkers(sensorData) {
        // Initialize the map with default coordinates for Colombo
        var map = L.map('map').setView([6.9271, 79.8612], 12);

        // Add a tile layer (OpenStreetMap in this case)
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(map);
        
        // Function to get pop-up color based on AQI
        function getColorForAQI(aqi) {
            let color, level, warning;
            if (aqi <= 50) {
                color = '#009865'; // Green - Good
                level = 'Good';
                warning = 'Air quality is good.';
            } else if (aqi <= 100) {
                color = '#c6aa15'; // Yellow - Moderate
                level = 'Moderate';
                warning = 'Air quality is acceptable; however, some pollutants may be a concern for a very small number of people who are sensitive to air pollution.';
            } else if (aqi <= 150) {
                color = '#ff9934'; // Orange - Unhealthy for Sensitive Groups
                level = 'Unhealthy for Sensitive Groups';
                warning = 'Air quality may be a concern for people with respiratory conditions.';
            } else if (aqi <= 200) {
                color = '#cc0033'; // Red - Unhealthy
                level = 'Unhealthy';
                warning = 'Everyone may begin to experience adverse health effects.';
            } else if (aqi <= 300) {
                color = '#7e0123'; // Purple - Very Unhealthy
                level = 'Very Unhealthy';
                warning = 'Health alert: everyone may experience more serious health effects.';
            } else {
                color = '#800000'; // Maroon - Hazardous
                level = 'Hazardous';
                warning = 'Health warning of emergency conditions. The entire population is more likely to be affected.';
            }
            return { color, level, warning };
        }


        // Helper function to format the date correctly
        function formatDate(dateString) {
            if (!dateString) {  // Check if dateString is null, undefined, or empty
                return 'Invalid date'; // Return a default value if the dateString is invalid
            }

            var timestamp = dateString.replace('/Date(', '').replace(')/', '');

            // Check if the timestamp is a valid number before creating the Date object
            var date = new Date(parseInt(timestamp));

            if (isNaN(date.getTime())) {
                return 'Invalid date';  // Return a default value if the date is invalid
            }

            return date.toLocaleDateString('en-GB');  // Format date (e.g., 'dd/mm/yyyy')
        }
        //function formatDate(dateString) {
        //    if (!dateString) {  // Check if dateString is null, undefined, or empty
        //        return 'Invalid date'; // Return a default value if the dateString is invalid
        //    }

        //    var timestamp = dateString.replace('/Date(', '').replace(')/', '');

        //    // Check if the timestamp is a valid number before creating the Date object
        //    var date = new Date(parseInt(timestamp));

        //    if (isNaN(date.getTime())) {
        //        return 'Invalid date';  // Return a default value if the date is invalid
        //    }

        //    return date.toLocaleDateString('en-GB');  // Format date (e.g., 'dd/mm/yyyy')
        //}



        // Loop through the sensor data and create markers
        sensorData.forEach(function (sensor) {
           
            // Create a marker for each sensor
            var marker = L.marker([sensor.SensorLatitude, sensor.SensorLongitude]).addTo(map);
            /*var marker = L.marker([sensor.SensorLatitude, sensor.SensorLongitude], { icon: markerIcon }).addTo(map);*/
            var aqi = sensor.Aqi || 0;
            var { color, level, warning } = getColorForAQI(aqi);
            // Format the date for pop-up content
            var dateFormatted = sensor.ReadingDate ? formatDate(sensor.ReadingDate) : 'No data';
            // Bind a popup to each marker
            // Customize the pop-up content with sensor details
            var popupContent = `
            <div style="background-color: ${color}; color: white; padding: 20px; border-radius: 5px;">
                <b>Location: </b>${sensor.SensorLocation || 'N/A'}<br>
                <b>Status: </b>${sensor.SensorStatus || 'N/A'}<br>                   
                <b>AQI: </b>${aqi} - <b>${level}</b><br>   
                <b>Warning: </b>${warning}<br>
                <b>PM2.5: </b>${sensor.PM2_5 || 'N/A'}<br>
                <b>PM10: </b>${sensor.PM10 || 'N/A'}<br>
                <b>PM1: </b>${sensor.Pm1 || 'N/A'}<br>
                <b>R.H: </b>${sensor.RH || 'N/A'}<br>
                <b>Temp: </b>${sensor.Temp || 'N/A'}<br>
                <b>Wind: </b>${sensor.Wind || 'N/A'}<br>
                <b>Date: </b>${dateFormatted}<br>
            </div>
        `;

            // Bind a popup to each marker
            marker.bindPopup(popupContent);
            // Call generateChart when a marker is clicked
            // Add a click event to generate the chart when a marker is clicked
            marker.on('click', function () {
                generateChart(sensor.SensorId);
            });
        });
        let currentChart = null; // Variable to hold the current chart
        function generateChart(sensorId) {
            $.ajax({
                url: '/DynamicData/GetAirQuality',
                type: 'GET',
                data: { sensorId: sensorId },
                success: function (data) {
                    if (data && data.length > 0) {
                        // If there's already a chart, destroy it before creating a new one
                        if (currentChart) {
                            currentChart.destroy();
                        }

                        // Prepare the data for the chart
                        var labels = data.map(item => formatDate(item.ReadingDate));
                        var pm25Data = data.map(item => item.PM2_5);
                        var pm10Data = data.map(item => item.PM10);
                        var pm1Data = data.map(item => item.Pm1);
                        var rhData = data.map(item => item.RH);
                        var tempData = data.map(item => item.Temp);
                        var windData = data.map(item => item.Wind);

                        // Create the chart using Chart.js
                        var ctx = document.getElementById('sensorChart').getContext('2d');
                        currentChart = new Chart(ctx, {
                            type: 'line',
                            data: {
                                labels: labels,
                                datasets: [
                                    {
                                        label: 'PM2.5',
                                        data: pm25Data,
                                        borderColor: 'rgba(0, 123, 255, 1)',
                                        fill: false
                                    },
                                    {
                                        label: 'PM10',
                                        data: pm10Data,
                                        borderColor: 'rgba(255, 99, 132, 1)',
                                        fill: false
                                    },
                                    {
                                        label: 'PM1',
                                        data: pm1Data,
                                        borderColor: 'rgba(54, 162, 235, 1)',
                                        fill: false
                                    },
                                    {
                                        label: 'RH',
                                        data: rhData,
                                        borderColor: 'rgba(75, 192, 192, 1)', 
                                        fill: false
                                    },
                                    {
                                        label: 'Temp',
                                        data: tempData,
                                        borderColor: 'rgba(255, 159, 64, 1)',
                                        fill: false
                                    },
                                    {
                                        label: 'Wind',
                                        data: windData,
                                        borderColor: 'rgba(153, 102, 255, 1)', 
                                        fill: false
                                    },

                                ]
                            },
                            options: {
                                responsive: true,
                                scales: {
                                    x: {
                                        beginAtZero: true
                                    },
                                    y: {
                                        beginAtZero: true
                                    }
                                }
                            }
                        });
                    } else {
                        alert("No data available for the selected sensor.");
                    }
                },
                error: function (error) {
                    console.error("Error fetching data: ", error);
                    alert("An error occurred while fetching data.");
                }
            });
        }
    }
});
