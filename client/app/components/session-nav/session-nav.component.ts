import { Component, OnInit, EventEmitter, Input, Output, SimpleChange } from '@angular/core';
import { CommonModule } from "@angular/common";
import { Http, Response } from "@angular/http";
import { ClientInfoService } from "../../services/clients.service"
import { ISession } from "../../types.d"

@Component({
    selector: 'session-nav',
    template: `
    <div class="container" [style.width]="isCollapsed ? '0' : '150px'" >   
        <div class="nav-top-spacer"></div>
        <ul>     
            <li *ngFor="let profile of profiles"
                    (click)="onClick(profile)"
                    (dblclick)="enterEdit(profile)"
                    class="navbar-item"
                    [class.active]="profile.selected">
                    
                    <input 
                        (blur)="doneEditingProfile(profile)"
                        [readonly]="!profile.isEditing"
                        [(value)]="profile.name"
                        [ngClass]="profile.isEditing ? 'editable-input' : 'readonly-input' "/>

                    </li>
            <li>
                <span class="add-profile-button navbar-item" 
                    (click)="addProfile($event, newProfileInput)"
                    *ngIf="!isAddingProfile" 
                    title="Add new profile">+</span>
                <input 
                    #newProfileInput
                    name="newProfileInput"
                    class="editable-input hidden"
                    [ngClass]="{'visible': isAddingProfile }"
                    [(ngModel)]="newProfileName" 
                    (blur)="doneEditingProfile()"/>

            </li>
        </ul>
    </div>
    `,
    styles: [` 
        input {
            outline: none;
        }

        .container {
            height: 100%;
            background-color: #3C4A54;
            color: #88959E;
            overflow: hidden;
            transition: width .25s ease;
        }

        input {
            font-family: 'Quicksand', sans-serif;
            width: 100%;
            height: 30px;
            padding-left: 10px;
            border: none;
            font-size: 16px;
        }

        .editable-input {
            transition: height .1s ease;
            background: #88959E;
        }

        .editable-input.hidden {
            width: 0;
            height: 0;
            opacity: 0;
        }

        .editable-input.visible {
            opacity: 1;
            width: 100%;
            height: 30px;
        }

        .readonly-input {
            background: transparent;
            color: #88959E;
            cursor: pointer;
        }

        .add-profile-button {
            display: none;
            border-radius: 15px;
            margin-left: 10px;
            padding-left: 9px;
            padding-right: 9px;
            cursor: pointer;
            border: none;
            outline: none;
            background: transparent;
            font-size: 24px;
        }

        .add-profile-button:hover {
            color: white;
            background: #36424B;
        }

        ul {
            list-style-type: none;
            margin: 0;
            padding: 0;
        }

        li.navbar-item { 
            padding-top: 4px;   
            padding-bottom: 4px;      
        }

        li.navbar-item:hover:not(.active) {
            background-color: #36424B;
        }

        li.active {
            background-color: #22303a;
            color: white;
        }

        .nav-top-spacer {
            height: 48px;
        }

    `],
    providers: [ClientInfoService]
})
export class ClientNavComponent implements OnInit {
    @Output() onSelected = new EventEmitter<ISession>();
    @Input() collapsed: boolean;

    private debugMessage: string;
    private selectedProfile: ISession;
    private profiles: ISession[];
    private isCollapsed: boolean;
    private isAddingProfile: boolean;
    private newProfileName: string;

    constructor(
        private clientsService: ClientInfoService) {
    }    

    private ngOnChanges(changes: { [propKey: string]: SimpleChange }) {
        for (let propName in changes) {
            let changedProp = changes[propName];

            if (propName == "collapsed" && changedProp.currentValue != undefined) {
                this.isCollapsed = changedProp.currentValue;
            }
        }
    }

    processKeyUp(e, el) {
        if(e.keyCode == 65) { // press A
        el.focus();
        }
    }

    ngOnInit() {
        this.profiles = [ {name: "BAX"}, {name:"BAX2"} ];
    }

    private enterEdit(profile) {
        profile.isEditing = true;
    }

    private doneEditingProfile(profile) {
        if (this.newProfileName ) {
            let newProfile = {name: this.newProfileName, path: ""};
            this.profiles.push(newProfile);
            this.isAddingProfile = false;
            this.newProfileName="";
            this.onClick(newProfile);
        }
        this.isAddingProfile = false;        
        if (profile) {
            profile.isEditing = false;
        }
    }

    private addProfile(e, newProfileInput) {
        this.isAddingProfile = true;
        newProfileInput.focus();
    }
    private onClick(profile) {
        this.debugMessage = profile.name;
        if (this.selectedProfile) {
            this.selectedProfile.selected = false;
        }
        this.selectedProfile = profile;
        this.selectedProfile.selected = true;
        this.onSelected.emit(profile);
    }
}
