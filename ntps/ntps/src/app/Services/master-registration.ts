import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { State } from '../models/common-models';

export interface MasterRegistrationModel {
  registrationType: string;
  nameTitle: string;
  name: string;
  emailId: string;
  loginId: string;
  password: string;
  mobileNo: string;
  isVerified: string; // since you're sending 'N' or 'Y'
  createdDate: string;
}

@Injectable({
  providedIn: 'root'
})
export class MasterRegistration {
  private apiUrl = 'https://localhost:7010/api/MasterRegistration';// Adjust if needed

  constructor(private http: HttpClient) { }

  register(data: MasterRegistrationModel): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }

  extractAadhaarData(fileData: FormData): Observable<any> {
  return this.http.post<any>('https://localhost:7010/api/MasterRegistration/ocr', fileData);
}

generateOtp(mobile: string) {
  return this.http.post('https://localhost:7010/api/MasterRegistration/generate-otp', JSON.stringify(mobile), {
    headers: { 'Content-Type': 'application/json' }
  });
}


verifyOtp(data: { mobileNumber: string; otp: string }) {
  return this.http.post('https://localhost:7010/api/MasterRegistration/verify-otp', data);
}

login(loginId: string, password: string): Observable<any> {
    return this.http.post('https://localhost:7010/api/MasterRegistration/login', { loginId, password });
  }

checkMobile(mobile: string) {
  return this.http.get(`https://localhost:7010/api/MasterRegistration/check-mobile/${mobile}`);
}

getAllStates(): Observable<State[]> {
    return this.http.get<State[]>(`https://localhost:7109/api/state/GetStates`);
  }

  getCirclesByState(stateId: number): Observable<any[]> {
  return this.http.get<any[]>(`https://localhost:7109/api/circle/GetCirclesByState/${stateId}`);
}

 getDivisionsByCircle(circleId: number): Observable<any[]> {
    return this.http.get<any[]>(`https://localhost:7109/api/Division/GetDivisionsByCircle/${circleId}`);
  }

  getRangesByDivision(divisionId: number): Observable<any[]> {
  return this.http.get<any[]>(`https://localhost:7109/api/Range/GetRangesByDivision/${divisionId}`);
}

getAadhaarDetails(aadhaarNumber: string) {
  return this.http.get<any>(`https://localhost:7010/api/MasterRegistration/fetch/${aadhaarNumber}`);
}

 saveApplicantDetails(formData: FormData): Observable<any> {
    return this.http.post(`https://localhost:7010/api/MasterRegistration/save`, formData);
  }

}
