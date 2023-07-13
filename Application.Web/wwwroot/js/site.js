function statistics() {
    $('#statistics_button').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        $.get('https://localhost:7282/api/statistics', function (data) {
            $('#total_Users').text(data.totalUsers + " Users");
            $('#total_Projects').text(data.totalProjects + " Projects");
            $('#total_Articles').text(data.totalArticles + " Articles");
            $('#statistics_box').removeClass('d-none');
            $('#statistics_button').hide();
        })
    })
}
