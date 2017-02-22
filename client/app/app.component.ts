import { Component } from '@angular/core';
import { Observable } from "rxjs/Observable";
import { ChannelService, ConnectionState } from "./services/channel.service";
import { IProfile } from "./types.d"

@Component({
    selector: 'my-app',
    template: `
    <div class="flex-columns">
        <div style="height: 30px; background: #cef; padding: 12px;">
            <strong>Service Helper Pro</strong>
        </div>
        <div class="flex-rows">
            <client-nav
                (onSelected)="onSelected($event)"
                ></client-nav>
            <simple-grid 
                [profile]="profile"
                ></simple-grid>
        </div>
    </div>
    `,
    styles: [` 
        .flex-columns { 
            display: flex;
            flex-direction: column;
            height: 100%;
        }
        .flex-rows { 
            display: flex;
            height: 100%;
        }
    `]
})
export class AppComponent {
    private connectionState$: Observable<string>;
    private profile: IProfile = { name:"", path: ""};

    constructor(
        private channelService: ChannelService
    ) {
        // Let's wire up to the signalr observables
        this.connectionState$ = this.channelService.connectionState$
            .map((state: ConnectionState) => { return ConnectionState[state]; });

        this.channelService.error$.subscribe(
            (error: any) => { console.warn(error); },
            (error: any) => { console.error("errors$ error", error); }
        );

        // Wire up a handler for the starting$ observable to log the
        //  success/fail result
        this.channelService.starting$.subscribe(
            () => { console.log("signalr service has been started"); },
            () => { console.warn("signalr service failed to start!"); }
        );
    }

    ngOnInit() {
        // Start the signalR connection
        this.channelService.start();
    }

    onSelected($event) {
        this.profile = $event;        
    }
}
