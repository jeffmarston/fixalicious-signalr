import { NgModule, Provider } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';

import { AgGridModule } from 'ag-grid-ng2/main';

import { AppComponent } from "./app.component";
import { SimpleGridComponent } from "./components/simple-grid.component";
import { ClientNavComponent } from "./components/client-nav/client-nav.component";
import { ProfileConfigComponent } from "./components/profile-config/profile-config.component";
import { ChannelService, ChannelConfig, SignalrWindow } from "./services/channel.service";
import { ClientInfoService} from "./services/clients.service";


let channelConfig = new ChannelConfig();
channelConfig.url = "/signalr";
channelConfig.hubName = "EventHub";

@NgModule({
    imports: [
        BrowserModule,
        HttpModule,
        FormsModule,
        AgGridModule.forRoot()
    ],
    declarations: [
        AppComponent,
        SimpleGridComponent,
        ClientNavComponent,
        ProfileConfigComponent
    ],
    providers: [ 
        ChannelService,
        ClientInfoService,
        { provide: SignalrWindow, useValue: window },
        { provide: "channel.config", useValue: channelConfig }
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
