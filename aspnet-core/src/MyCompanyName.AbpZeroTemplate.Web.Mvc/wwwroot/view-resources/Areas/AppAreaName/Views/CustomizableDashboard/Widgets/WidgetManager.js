var app = app || {};
(function ($) {
  app.widgets = app.widgets || {};
  
  app.WidgetManager = (function () {
    function _clearContainer(widgetSelector) {
      var $container = $(widgetSelector);
      if ($container.length) {
        $container.empty();
      }
    }

    return function (options) {
      var _options = options;
      var _$widgetContainer = null;
      var _widgetContainerSelector = '#' + options.containerId;
      var _widgetObject = null;

      var _publicApi = null;
      var _args = null;

      var _initted = false;
      
      var _delayedRunner = app.utils.widgets.delayedRunner.create();

      function initAndShowWidget() {
        if (_initted) {
          return;
        }
        _initted = true;

        _$widgetContainer = $(_widgetContainerSelector);

        _clearContainer(_widgetContainerSelector);

        _$widgetContainer.load(options.viewUrl, _args, function (response, status, xhr) {
          if (status === 'error') {
            abp.message.warn(abp.localization.abpWeb('InternalServerError'));
            return;
          }

          if (options.scriptUrl) {
            let basePath = abp.appPath === '/' ? '' : abp.appPath;
            app.ResourceLoader.loadScript(basePath + options.scriptUrl, function () {
              let widgetClass = app.widgets[options.widgetClass];
              if (!widgetClass) {
                return
              }
              
              _widgetObject = new widgetClass();
              if (typeof _widgetObject.init === 'function') {
                _widgetObject.init(_publicApi);
              }
              
              let shouldWatchForResize = (typeof _widgetObject.onResizeCompleted === 'function') || (typeof _widgetObject.onResizeStarted === 'function');
              if(shouldWatchForResize){
                let gridStackItem = _$widgetContainer.closest('.grid-stack-item');
                if (gridStackItem.length) {
                  new GridStackItemResizeWatcher(gridStackItem[0], _widgetObject.onResizeStarted, _widgetObject.onResizeCompleted);
                }
              }
              
            });
          }
        });
      }

      function initialize() {
        let closestPage = $('#' + options.containerId).closest('.tab-pane');
        if (closestPage.hasClass('active')) {
          //if the widget is in active tab, load it immediately
          initAndShowWidget();
        } else {
          //otherwise wait for the tab to be active for the widget to load
          var closesPagesId = closestPage.attr('id');

          $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
            var target = $(e.target).attr('href');
            if (target === '#' + closesPagesId) {
              //if widget's page is activated, load widget
              initAndShowWidget();
            }
          });
        }
      }

      _publicApi = {
        getWidgetContainerId: function () {
          return _options.containerId;
        },

        getWidget: function () {
          return _$widgetContainer;
        },

        getOptions: function () {
          return _options;
        },

        getWidgetObject: function () {
          return _widgetObject;
        },
        
        runDelayed: function (callback) {
          _delayedRunner.run(callback);
        }
      };

      initialize();

      return _publicApi;
    };
  })();
})(jQuery);
