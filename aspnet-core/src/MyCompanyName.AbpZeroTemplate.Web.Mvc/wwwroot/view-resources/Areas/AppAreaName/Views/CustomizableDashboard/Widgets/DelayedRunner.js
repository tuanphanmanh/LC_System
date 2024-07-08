(function () {
  app.utils.widgets = app.utils.widgets || {};
  
  app.utils.widgets.delayedRunner = (function () {
    return function () {
      let delay = 300;
      let timer;

      var _runDelayed = function (callBack) {
        if (timer) {
          clearTimeout(timer);
        }

        timer = setTimeout(function () {
          callBack();
        }, delay);
      };
      
      return {
        run: _runDelayed,
      };
    };
  })();

  app.utils.widgets.delayedRunner.create = function () {
    return new app.utils.widgets.delayedRunner();
  };
})();
