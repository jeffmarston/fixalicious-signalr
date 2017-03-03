import { Component, OnInit, EventEmitter, Input, Output, SimpleChange } from '@angular/core';
import { CommonModule } from "@angular/common";
import { Http, Response } from "@angular/http";
import { SessionService } from "../../services/session.service"
import { TransactionApiService } from "../../services/transaction.service"
import { ISession, IFixMessage, ITransaction } from "../../types.d"

@Component({
    selector: 'detail-pane',
    template: `
    <div class="container" [style.width]="collapsed ? '0' : '280px'" > 
        <div class="button-section">
            <button [disabled]="!isValid" (click)="ackFixMessage()">Ack</button>
            <button [disabled]="true">Reject</button>
            <button [disabled]="true">Fill</button>
            <button [disabled]="true">Partial Fill</button>
        </div>
        <div class="keyvalue-section">
            <table class="keyvalue-table">
                <tr *ngFor="let pair of kvPairs" >
                    <td>{{pair.key}}</td>
                    <td><input [(ngModel)]="pair.value"/></td>
                </tr>
            </table>
        </div>
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

        .button-section {
            display: flex;
            flex-direction: column;
        }

        .keyvalue-section {
            overflow: auto;
            border-collapse: collapse;
            margin-left: 6px;
        }

        .keyvalue-table {
            font-size: 12px;
        }

        button { 
            padding: 4px;
            margin: 0 6px 6px 6px;    
        }

        li:hover:not(.active) {
            background-color: #36424B;
        }

    `],
    providers: [SessionService]
})
export class DetailPane implements OnInit {
    @Input() detail: ITransaction;
    @Input() collapsed: boolean;
    @Input() session: ISession;

    private transaction: ITransaction;
    private isValid: boolean;
    private kvPairs: any[] = [];

    constructor(
        private clientsService: SessionService,
        private apiDataService: TransactionApiService) {
                this.isValid = true;
    }    

    private ngOnChanges(changes: { [propKey: string]: SimpleChange }) {
        for (let propName in changes) {
            let changedProp = changes[propName];

            if (propName == "detail" && changedProp.currentValue != undefined) {
                this.transaction = changedProp.currentValue;
                this.displayFixMessage();
                this.isValid = true;
            }
            if (propName == "collapsed" && changedProp.currentValue != undefined) {
                this.collapsed = changedProp.currentValue;
            }
        }
    }

    private displayFixMessage() {
        this.kvPairs = [];
        var fixMsg = JSON.parse(this.transaction.message);
        for (var property in fixMsg) {
            if (fixMsg.hasOwnProperty(property)) {
                this.kvPairs.push({ 
                    key: property, 
                    value: fixMsg[property]});
            }
        }
    }

    private ackFixMessage() {
        var fixObj = {};

        this.kvPairs.forEach(pair => {
            fixObj[pair.key] = pair.value;
        });

        console.log(fixObj);
        alert(JSON.stringify(fixObj));

        this.apiDataService.createTransaction(this.session.name, fixObj);
    }

    ngOnInit() {
        
    }

}
