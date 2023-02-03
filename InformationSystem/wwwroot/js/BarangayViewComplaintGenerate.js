getChartsData();
function getChartsData() {
    var healthChart = document.getElementById("healthChart").getContext('2d');
    var disease = ["Crime %","Health %"];
    var crime = [];

    $.ajax({
        url: "/Complaints/GenerateDataReport?complaintReport=" + $("#report").val(),
        type: "POST",
        success: function (data) {
            var myChart = new Chart(healthChart, {
                type: 'pie',
                data: {
                    labels: disease,
                    datasets: [{
                        fill: false,
                        backgroundColor: [
                            '#022859',
                            '#5C8EF2'],
                        borderCapStyle: 'butt',
                        data: data,
                    }]
                },
                options: {
                    responsive: true,
                    hover: {
                        mode: 'label'
                    },
                    plugins: {
                        legend: {
                            labels: {
                                // This more specific font property overrides the global property
                                font: {
                                    size: 124
                                }
                            }
                        }
                    }
                }
            });
            myChart.defaults.font.size = 16;
        }
    });
}