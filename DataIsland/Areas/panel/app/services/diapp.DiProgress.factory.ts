declare function show_loading_bar(progress:number);

module diapp.services {
    'use strict';

    export interface IDiProgress {
        addRequest(): void;
        removeRequest(): void;
    }

    class DiProgress implements IDiProgress {
        numOfRequests: number = 0;

        addRequest():void {
            this.numOfRequests++;
            this.showLoadingBar();
        }

        removeRequest():void {
            this.numOfRequests--;
            if (this.numOfRequests < 0) {
                this.numOfRequests = 0;
            }
            this.showLoadingBar();
        }

        showLoadingBar():void {
            if (this.numOfRequests > 0) {
                try
                {
                    show_loading_bar(100 / (this.numOfRequests + 1));
                }
                catch (e) {

                }
            }
            if (this.numOfRequests == 0) {
                try {
                    show_loading_bar(100);
                }
                catch (e) {

                } 
            }
        }
    } 

    function factory(): IDiProgress {
        return new DiProgress();
    }

    angular
        .module('diapp')
        .factory('diapp.services.DiProgress',
        factory); 
}