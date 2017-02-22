import { Component, OnInit, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from "@angular/common";
import { Http, Response } from "@angular/http";
import { ClientInfoService } from "../../services/clients.service"
import { IProfile } from "../../types.d"

@Component({
    selector: 'client-nav',
    template: `
    <div class="wrapper">        
        <select name="sometext" multiple="multiple">
            <option *ngFor="let profile of profiles; let i = index;"
                    (click)="onClicky(profile)"
                    [selected]="i == 0"
                    >{{profile.name}}</option>
        </select>
    </div>
    `,
    styles: [` 
        .wrapper {
            height: 100%;
            background: azure;
        }

        select { 
            border: none;
            width: 200px;
            height: 100%;
            overflow: auto;
            background: transparent;
        }

        select>option {
            padding: 8px 5px;
        }

    `],
    providers: [ClientInfoService]
})
export class ClientNavComponent implements OnInit {
    @Output() onSelected = new EventEmitter<IProfile>();

    private debugMessage: string;
    private profiles: IProfile[];


    constructor(
        private clientsService: ClientInfoService) {
    }

    ngOnInit() {
        var src = this.clientsService.getClients();
        src.subscribe(o => {
            this.profiles = o;
            if (this.profiles.length > 0){
                this.onSelected.emit(this.profiles[0]);
            }

        }, error => {
            console.error("ERROR: " + error);
        });
    }

    private onClicky(profile) {
        this.debugMessage = profile.name;
        this.onSelected.emit(profile);
    }
}
