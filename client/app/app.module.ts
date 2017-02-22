import { NgModule, Provider } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpModule } from '@angular/http';
import { FormsModule } from '@angular/forms';

import { AgGridModule } from 'ag-grid-angular/main';

import { AppComponent } from "./app.component";
import { SimpleGridComponent } from "./components/message-grid/message-grid.component";
import { ClientNavComponent } from "./components/session-nav/session-nav.component";
import { DetailPane } from "./components/detail-pane/detail-pane.component";
import { ProfileConfigComponent } from "./components/profile-config/profile-config.component";
import { ClientInfoService } from "./services/clients.service";

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
        DetailPane,
        ProfileConfigComponent
    ],
    providers: [ 
        ClientInfoService
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
