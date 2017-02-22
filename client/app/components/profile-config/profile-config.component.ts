import { Component, OnInit, EventEmitter, Input, Output, SimpleChange } from '@angular/core';
import { CommonModule } from "@angular/common";
import { Http, Response } from "@angular/http";
import { IProfile } from "../../types.d"

@Component({
    selector: 'profile-config',
    templateUrl: "app/components/profile-config/profile-config.component.html",
    styleUrls: ["app/components/profile-config/profile-config.component.css"]
})
export class ProfileConfigComponent {
    @Output() onSave = new EventEmitter<IProfile>();
    @Input() profile: IProfile;

    private debugMessage: string;
    private hideForm: boolean = true;

    constructor() {
    }

    private onClicky(profile) {
        this.onSave.emit(profile);
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
