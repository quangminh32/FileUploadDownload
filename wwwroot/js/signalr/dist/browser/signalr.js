// SignalR Client Library - Basic Implementation
// This is a simplified version for development purposes

(function (global, factory) {
    typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
    typeof define === 'function' && define.amd ? define(['exports'], factory) :
    (global = global || self, factory(global.signalR = {}));
}(this, function (exports) {
    'use strict';

    var signalR = {
        HubConnectionBuilder: function() {
            return {
                withUrl: function(url) {
                    this.url = url;
                    return this;
                },
                build: function() {
                    var connection = {
                        url: this.url,
                        on: function(method, callback) {
                            this.handlers = this.handlers || {};
                            this.handlers[method] = callback;
                        },
                        start: function() {
                            console.log('SignalR connection started to:', this.url);
                            return Promise.resolve();
                        },
                        invoke: function(method, ...args) {
                            console.log('Invoking method:', method, 'with args:', args);
                            return Promise.resolve();
                        }
                    };
                    return connection;
                }
            };
        }
    };

    exports.HubConnectionBuilder = signalR.HubConnectionBuilder;
}));