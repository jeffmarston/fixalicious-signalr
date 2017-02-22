import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { Observable } from 'rxjs/Rx';

@Injectable()
export class ClientInfoService {
    private baseurl = "/serviceInfo/profile";

    constructor(private http: Http) {
    }

    private pollForServices() {
        Observable;//.create();
    }

    public getClients(): Observable<any> {
        return this.http.get(this.baseurl)
            .map(res => res.json());
    }
}