import { Component, OnInit, Input, OnChanges, SimpleChange } from '@angular/core';
import { CommonModule } from "@angular/common";
import { Http, Response } from "@angular/http";
import { GridOptions } from 'ag-grid/main';
import { ServiceInfoService } from "../services/serviceinfo.service"
import { ChannelService, ChannelEvent } from "../services/channel.service"
import { StatusCellComponent } from "./statusCell.component"
import { IServiceInfo, IProfile } from "../types.d"


@Component({
    selector: 'simple-grid',
    templateUrl: 'app/components/simple-grid.component.html',
    styleUrls: ['app/components/simple-grid.component.css'],
    providers: [ServiceInfoService]
})
export class SimpleGridComponent implements OnInit {
    @Input() profile: IProfile;

    private gridOptions: GridOptions;
    private showGrid: boolean;
    private rowData: IServiceInfo[];
    private columnDefs: any[];
    private selectedProfile: IProfile;

    private debugMessage: string;

    ngOnInit() {
        // Get an observable for events emitted on this channel
        this.channelService.sub("serviceInfo").subscribe(
            (x: ChannelEvent) => {
                switch (x.Name) {
                    case "serviceInfo.status": {
                        this.updateDataSource(x.Data);
                        console.log(x.Data[0].name + " - " + x.Data[0].status);
                    }
                }
            },
            (error: any) => {
                console.warn("Attempt to join channel failed!", error);
            }
        );

        this.showGrid = true;
    }

    constructor(
        private apiDataService: ServiceInfoService,
        private http: Http,
        private channelService: ChannelService) {

        this.createColumnDefs();
        // we pass an empty gridOptions in, so we can grab the api out
        this.gridOptions = <GridOptions>{
            columnDefs: this.columnDefs
        };
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

    private createColumnDefs() {
        this.columnDefs = [
            { headerName: "Name", field: "name" },
            {
                headerName: "Status", field: "status", cellRendererFramework: {
                    component: StatusCellComponent,
                    moduleImports: [CommonModule]
                }
            },
            { headerName: "PID", field: "pid" }
        ];
    }

    private createRowData(profile: IProfile) {
        var src = this.apiDataService.getServices(profile.name);
        src.subscribe(o => {
            this.updateDataSource(o);
        }, error => {
            console.error("ERROR: " + error);
        });
    }

    private updateDataSource(changes: IServiceInfo[]) {
        if (!this.rowData) {
            this.rowData = changes;
        }

        // look for updates
        let updatedNodes = [];
        this.gridOptions.api.forEachNode(function (node) {
            var data = node.data;
            let newValue = changes.find(o => o.name == data.name);
            if (newValue) {
                if (data.status !== newValue.status) {
                    data.status = newValue.status;
                }
                if (data.pid !== newValue.pid) {
                    data.pid = newValue.pid;
                }
                updatedNodes.push(node);
            }
        });

        // Refresh the grid
        this.gridOptions.api.refreshCells(updatedNodes, ['status', 'pid']);
    }

    private startStopService($event, action: string) {
        let nodes = this.gridOptions.api.getSelectedNodes();
        nodes.forEach(o => {
            let svc = o.data;
            this.apiDataService.sendCommand(svc.name, action);
        });
    }

    private onCellDoubleClicked($event) {
        let svc = this.rowData[$event.rowIndex];
        if (svc.status === "Running") {
            this.apiDataService.sendCommand(svc.name, "stop");
        } else if (svc.status === "Stopped") {
            this.apiDataService.sendCommand(svc.name, "start");
        }else if (svc.status === "") {
            this.apiDataService.sendCommand(svc.name, "install");
        }
    }
}
