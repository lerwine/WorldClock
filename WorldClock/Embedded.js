var _WPQ_Timer;
var _WPQ_Table;

function _WPQ_FormatNumber(numberVal) {
    if (numberVal < 10)
        numberVal = "0" + numberVal;
        
    return numberVal;
}
function _WPQ_Update() {
    var table;
    var today;
    var tz;
    
    today = new Date();
    
    for (var c=0; c<_WPQ_TimeZones.length; c++) {
        if ((c * 2) < _WPQ_Table.rows[1].cells.length) {
            tz = new Date(Date.UTC(today.getUTCFullYear(), today.getUTCMonth(), today.getUTCDate(), today.getUTCHours(), 
                today.getUTCMinutes(), today.getUTCSeconds(), today.getUTCMilliseconds()) + 
                (_WPQ_TimeZones[c] * 1000));
            _WPQ_Table.rows[1].cells[c * 2].innerText = _WPQ_FormatNumber(tz.getHours()) + ":" +
                    _WPQ_FormatNumber(tz.getMinutes()) + ":" + _WPQ_FormatNumber(tz.getSeconds());
        }
    }
}
function _WPQ_Tick() {
    clearTimeout(_WPQ_Timer);
    _WPQ_Update();
    _WPQ_Timer = setTimeout("_WPQ_Tick()", 1000);
}

function _WPQ_Init(tableid) {
    if ((_WPQ_Table = document.getElementById(tableid)) == null || _WPQ_Table.rows.length < 2) {
        alert("Timer table not found.");
        return;
    }

    _WPQ_Update();
    _WPQ_Timer = setTimeout("_WPQ_Tick()", 1000);
}
