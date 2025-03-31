$(document).ready(function () {
    //Fetach the dashboard data from AdminfeaturesController
    $.ajax({
        url: '/AdminFeatures/DashBoardCount',
        type: 'GET',
        success: function (data) {
            if (data.error) {
                alter('Error' + data.error);
            } else {
                $('#activeSensors').text(data.ActiveSensors);
                $('#inActiveSensors').text(data.InActiveSensors);
                $('#totalUsers').text(data.TotalUsers);
                $('#adminAcc').text(data.AdminUsers);

                //call the function to render the bar chart
                renderSensorBarChart(data.ActiveSensors, data.InActiveSensors);
            }

        },
        error: function (xhr, status, error) {
            console.error("Error fetching data: " + error);
        }
    });
});
//Function to render the bar chart using chartJS
function renderSensorBarChart(active, inactive) {
    //create data for the bar cahrt
    var chartData = {
        // X- axis labels
        labels: ['Active Sensors', 'Inactive Sensors'],
        datasets: [{
            //chart label
            label: 'Sensor Status',
            //Data for each bar
            data: [active, inactive],
            // Colors for the bars (green for active, red for inactive)
            backgroundColor: ['#4CAF50', '#FF5722']
        }]
    };
    //create the bar chart
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
    })
}