import { Component } from '@angular/core';

@Component({
    template: `<div [ngClass]="class">{{text}}</div>`
})
export class StatusCellComponent {
    private text: any;
    private class: string;

    agInit(params: any): void {
        this.refresh(params);
    }

    refresh(params: any) {
        this.text = params.value;
        if (params.value == "Running") {
            this.class = "service-status running";
        } else if (params.value == "Stopped") {
            this.class = "service-status stopped"; 
        } else if (params.value == "") {
            this.class = "service-status none";
        } else {
            this.class = "service-status unknown";
        }
    }
}