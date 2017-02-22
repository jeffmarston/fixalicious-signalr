import { Component, OnInit, EventEmitter, Input, Output, SimpleChange } from '@angular/core';
import { CommonModule } from "@angular/common";
import { Http, Response } from "@angular/http";
import { ISession } from "../../types.d"

@Component({
    selector: 'profile-config',
    templateUrl: "app/components/profile-config/profile-config.component.html",
    styleUrls: ["app/components/profile-config/profile-config.component.css"]
})
export class ProfileConfigComponent {
    @Output() onSave = new EventEmitter<ISession>();
    @Input() profile: ISession;

    private debugMessage: string;
    private hideForm: boolean = true;

    constructor() {
        this.profile = {name:""};
    }

    private onClick() {
        this.onSave.emit(this.profile);
    }

    private toggleCollapsed() {
        this.hideForm = !this.hideForm;
    }

    private ngOnChanges(changes: { [propKey: string]: SimpleChange }) {
        for (let propName in changes) {
            let changedProp = changes[propName];

            if (propName == "profile" && changedProp.currentValue != undefined) {
                this.profile = changedProp.currentValue;
                
            }
        }
    }

}
