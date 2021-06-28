$(document).ready(function () {
    $('body').on('click', 'a', function (e) {
        e.preventDefault();
        invokeCSCode(e.target.href);
    })
});
function invokeCSCode(data) {
    try {
        invokeCSharpAction(data);
    } catch (err) {
    }
}
