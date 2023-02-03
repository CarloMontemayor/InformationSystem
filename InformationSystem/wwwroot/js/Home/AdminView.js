getChartsData();
function getChartsData() {
    var healthChart = document.getElementById("healthChart").getContext('2d');
    var fundChart = document.getElementById("crimeChart").getContext('2d');
    var disease = [];
    var crime = [];
    $.ajax({
        url: "/AdminView/GetDisease",
        type: "POST",
        success: function (result) {
            console.log(result);
            disease = result;

            $.ajax({
                url: "/AdminView/GetDiseaseData",
                type: "POST",
                success: function (data) {
                    var myChart = new Chart(healthChart, {
                        type: 'bar',
                        data: {
                            labels: disease,
                            datasets: [{
                                fill: false,
                                backgroundColor: "#022859",
                                borderColor: "#022859",
                                borderCapStyle: 'butt',
                                data: data,
                            }]
                        },
                        options: {
                            responsive: true,
                            legend: {
                                display: false,
                            },
                            hover: {
                                mode: 'label'
                            },
                            scales: {
                                yAxes: [{
                                    ticks: {
                                        beginAtZero: true,
                                        stepSize: 1
                                    }
                                }]
                            }
                        }
                    });
                }
            });
        }
    });

    $.ajax({
        url: "/AdminView/GetCrime",
        type: "POST",
        success: function (result) {
            console.log(result);
            crime = result;

            $.ajax({
                url: "/AdminView/GetCrimeData",
                type: "POST",
                success: function (data) {
                    var myChart = new Chart(fundChart, {
                        type: 'bar',
                        data: {
                            labels: crime,
                            datasets: [{
                                fill: false,
                                backgroundColor: "#5C8EF2",
                                borderColor: "#5C8EF2",
                                borderCapStyle: 'butt',
                                data: data,
                            }]
                        },
                        options: {
                            responsive: true,
                            legend: {
                                display: false,
                            },
                            hover: {
                                mode: 'label'
                            },
                            scales: {
                                yAxes: [{
                                    ticks: {
                                        beginAtZero: true,
                                        stepSize: 1
                                    }
                                }]
                            }
                        }
                    });
                }
            });
        }
    });
}

$(document).ready(function () {
    getCardsData();
});


function getCardsData() {

    $.ajax({
        url: "/AdminView/GetTeenAgerData",
        type: "POST",
        success: function (result) {
            console.log(result);
            $('#teenager').text(result);
        }
    });

    $.ajax({
        url: "/AdminView/GetAdultData",
        type: "POST",
        success: function (result) {
            console.log(result);
            $('#adult').text(result);
        }
    });

    $.ajax({
        url: "/AdminView/GetSeniorData",
        type: "POST",
        success: function (result) {
            console.log(result);
            $('#senior').text(result);
        }
    });
}