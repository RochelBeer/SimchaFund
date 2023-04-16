$(() => {

    console.log("simcha page loaded!!")
    $("#new-simcha").on('click', function () {
        console.log('new simcha')
        new bootstrap.Modal($("#add-simcha-modal")[0]).show();
    })

    //$("#new-simcha").on('click', function () {
    //    console.log('new simcha')
    //    new bootstrap.Modal($(".modal fade")[0]).show();
    //})



})