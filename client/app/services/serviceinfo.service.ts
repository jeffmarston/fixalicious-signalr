import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { Observable } from 'rxjs/Rx';

@Injectable()
export class ServiceInfoService {
    private baseurl = "/serviceInfo/service";

    constructor(private http: Http) {
    }

    private pollForServices() {
        Observable;//.create();
    }

    public getServices(profile: string): Observable<any> {
        return this.http.get(this.baseurl + "/" + profile)
            .map(res => res.json());
    }

    public sendCommand(svcId: string, action: string) {
        let url = `${this.baseurl}/${svcId}?action=${action}`;
        this.http.post(url, "")
            .subscribe(o => { });
    }
}