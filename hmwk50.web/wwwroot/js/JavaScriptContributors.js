$(() => {

    console.log("page loaded!!")
    $("#new-contributor").on('click', function () {
        Reset();
        new bootstrap.Modal($("#new-contributor-modal")[0]).show();
    })


    $(".deposit-button").on('click', function () {
        const id = $(this).data('contributorid');
        $("#contributorId").val(id);
        const name = $(this).data('name')
        $("#deposit-name").text(name)

        new bootstrap.Modal($(".deposit")[0]).show();

    })

    $(".edit-contributor").on('click', function () {
        const button = $(this);
        $(".modal-title-contributor").text("Edit Contributor")
        $("#contributor_first_name").val(button.data("first-name"))
        $("#contributor_last_name").val(button.data("last-name"))
        $("#contributor_cell_number").val(button.data("cell"))
        $("#contributor_created_at").val(button.data("date"))
        console.log(button.data("date"))
        $("#initialDepositDiv").hide()
        $("form").attr("action", "/contributors/editContributor")

        let checkBox = button.data("always-include");
        if (checkBox === "True") {
            $("#contributor_always_include").prop("checked", true)
        }
        else {
            $("#contributor_always_include").prop("checked", false)
        }
        let id = button.data("id");
        $("#hiddenId").remove()
        $(".modal-body").append(` <input name="Id" type="hidden" value="${id}" id="hiddenId">`)

        new bootstrap.Modal($("#new-contributor-modal")[0]).show();

    })

    function Reset() {
        $(".modal-title-contributor").text("New Contributor")
        $("#contributor_first_name").val("")
        $("#contributor_last_name").val("")
        $("#contributor_cell_number").val("")
        $("#contributor_created_at").val("")
        $("#hiddenId").remove()
        $("#initialDepositDiv").show()
    }
    
    $("#search").on("input", function () {
        var searchText = $("#search").val().toLowerCase();
        $(".tr").filter(function () {
            return $("#search").text().toLowerCase().indexOf(searchText) === -1;
        }).hide();
        $(".tr").filter(function () {
            return $(this).text().toLowerCase().indexOf(searchText) !== -1;
        }).show();

    });
    $("#clear").on('click', function () {
        $("#search").val("");
        $(".tr").show();
    })



})

