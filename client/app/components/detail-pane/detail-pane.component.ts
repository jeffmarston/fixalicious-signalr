import { Component, OnInit, EventEmitter, Input, Output, SimpleChange } from '@angular/core';
import { CommonModule } from "@angular/common";
import { Http, Response } from "@angular/http";
import { ClientInfoService } from "../../services/clients.service"
import { ServiceInfoService } from "../../services/serviceinfo.service"
import { ISession, IFixMessage } from "../../types.d"

@Component({
    selector: 'detail-pane',
    template: `
    <div class="container" [style.width]="collapsed ? '0' : '220px'" > 
        <button [disabled]="!isValid" (click)="ackFixMessage()">Ack</button>
        <button [disabled]="!isValid">Reject</button>
        <button [disabled]="!isValid">Fill</button>
        <button [disabled]="!isValid">Partial Fill</button>
    </div>
    `,
    styles: [` 
        
        .container {
            height: 100%;
            overflow: hidden;
            transition: width .25s ease;
            display: flex;
            flex-direction: column;
        }

        button { 
            padding: 4px;
            margin: 6px;    
        }

        li:hover:not(.active) {
            background-color: #36424B;
        }

    `],
    providers: [ClientInfoService]
})
export class DetailPane implements OnInit {
    @Input() detail: IFixMessage;
    @Input() collapsed: boolean;

    private fixMessage: IFixMessage;
    private fixMsgJson: string;
    private isValid: boolean;

    constructor(
        private clientsService: ClientInfoService,
        private apiDataService: ServiceInfoService) {
    }    

    private ngOnChanges(changes: { [propKey: string]: SimpleChange }) {
        for (let propName in changes) {
            let changedProp = changes[propName];

            if (propName == "detail" && changedProp.currentValue != undefined) {
                this.fixMessage = changedProp.currentValue;
                this.fixMsgJson = JSON.stringify(this.fixMessage);
                this.isValid = true;
            }
            if (propName == "collapsed" && changedProp.currentValue != undefined) {
                this.collapsed = changedProp.currentValue;
            }
        }
    }

    private ackFixMessage() {
        this.apiDataService.sendCommand("BAX", "");
    }

    ngOnInit() {
        
    }

}
