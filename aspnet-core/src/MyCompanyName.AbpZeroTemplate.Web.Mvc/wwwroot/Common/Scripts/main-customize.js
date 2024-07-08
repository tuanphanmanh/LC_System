(function () {
    abp.libs.spinjs = {
        spinner_config: {
            animation: 'spinner-line-fade-quick', // The CSS animation name for the lines
            direction: 1, // 1: clockwise, -1: counterclockwise
            color: '#ffffff', // CSS color or array of colors
            fadeColor: 'transparent', // CSS color or array of colors
            shadow: '0 0 1px transparent', // Box-shadow for the lines
            zIndex: 2000000000, // The z-index (defaults to 2e9)
            className: 'spinner', // The CSS class to assign to the spinner
            position: 'absolute', // Element positioning,		
            lines: 8, // The number of lines to draw
            length: 2, // The length of each line
            width: 10, // The line thickness
            radius: 25, // The radius of the inner circle
            corners: 5, // Corner roundness 
        }
    };
})();