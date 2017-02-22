import { Component, OnInit, Input, OnChanges, SimpleChange } from '@angular/core';
import { CommonModule } from "@angular/common";
import { Http, Response } from "@angular/http";
import { GridOptions } from 'ag-grid/main';
import { ServiceInfoService } from "../../services/serviceinfo.service"
import { ClientInfoService } from "../../services/clients.service"
import { IFixMessage, ISession } from "../../types.d"


@Component({
    selector: 'message-grid',
    templateUrl: 'app/components/message-grid/message-grid.component.html',
    styleUrls: ['app/components/message-grid/message-grid.component.css'],
    providers: [ServiceInfoService]
})
export class SimpleGridComponent implements OnInit {
    @Input() profile: ISession;

    private gridOptions: GridOptions;
    private showGrid: boolean;
    private rowData: IFixMessage[];
    private columnDefs: any[];
    private selectedProfile: ISession;
    private selectedMessage: IFixMessage;
    private showDetails: boolean;

    private debugMessage: string;

    ngOnInit() {
        this.showGrid = true;
    }

    constructor(
        private apiDataService: ServiceInfoService,
        private profileService: ClientInfoService,
        private http: Http) {

        this.createColumnDefs();
        // we pass an empty gridOptions in, so we can grab the api out
        this.gridOptions = <GridOptions>{
            columnDefs: this.columnDefs
        };
        //this.rowData=[];
    }

    private ngOnChanges(changes: {[propKey: string]: SimpleChange}) {
        for (let propName in changes) {
            let changedProp = changes[propName];

            if (propName == "profile") {
                this.selectedProfile = changedProp.currentValue;
                this.rowData = null;
                this.createRowData(changedProp.currentValue);
            }
        }
    }

    private saveProfile(profile: ISession) {  
        this.profileService.createProfile(profile);
        this.rowData = null;
        this.createRowData(profile);
    }

    private createColumnDefs() {
        this.columnDefs = [
            { headerName: "Direction", field: "direction" },
            { headerName: "Symbol", field: "symbol" },
            { headerName: "LastShares", field: "lastShares" },
            { headerName: "LastPx", field: "lastPx" },
            { headerName: "ClOrdId", field: "clOrdId" }
        ];
    }

    private createRowData(profile: ISession) {

        // setTimeout( o=> {
        //     this.updateDataSource([ 
        //         { direction: "-> Received",
        //           symbol: "AAPL",
        //           clOrdId: "ABC123",
        //           lastShares: 100,
        //           lastPx: 123.4,
        //           side: 0 
        //         },
        //         { direction: "-> Sent",
        //           symbol: "AAPL",
        //           clOrdId: "ABC123",
        //           lastShares: 30,
        //           lastPx: 123.4,
        //           side: 0 
        //         },
        //         { direction: "-> Received",
        //           symbol: "GE",
        //           clOrdId: "ABC123",
        //           lastShares: 3000,
        //           lastPx: 34.4,
        //           side: 0 
        //         }
        //     ]);
        // }, 1000);
        
        var src = this.apiDataService.getServices(profile.name);
        src.subscribe(o => {
            this.updateDataSource(o);
        }, error => {
            console.error("ERROR: " + error);
        });
    }

    private updateDataSource(changes: IFixMessage[]) {
        if (!this.rowData) {
            this.rowData = changes;
        }

        // look for updates
        let updatedNodes = [];
        this.gridOptions.api.forEachNode(function (node) {
            var data = node.data;
            let newValue = changes.find(o => o.clOrdId == data.clOrdId);
            if (newValue) {
                updatedNodes.push(node);
            }
        });

        // Refresh the grid
        this.gridOptions.api.refreshCells(updatedNodes, ['status', 'pid']);
    }

    private onCellDoubleClicked($event) {
        this.selectedMessage = $event.data;
        //this.showDetails = !this.showDetails;
    }
}
