document.body.addEventListener('htmx:confirm', function (evt) {
    if (evt.target.matches("[confirm-with-sweet-alert='true']")) {
        evt.preventDefault();
        swal({
            title: "Are you sure?",
            text: "Are you sure you are sure?",
            icon: "warning",
            buttons: true,
            dangerMode: true,
        }).then((confirmed) => {
            if (confirmed) {
                evt.detail.issueRequest();
            }
        });
    }
});
