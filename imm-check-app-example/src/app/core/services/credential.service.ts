import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class CredentialService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // Credential management
  issueCredential(request: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/credential/issue`, request);
  }

  getCredential(id: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/credential/${id}`);
  }

  getCredentialsBySubject(subjectDid: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/credential/subject/${encodeURIComponent(subjectDid)}`);
  }

  verifyCredential(credential: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/credential/verify`, { credential });
  }

  revokeCredential(id: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/credential/${id}/revoke`, {});
  }

  getSchemas(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/credential/schemas`);
  }

  // DID management
  createDid(method: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/did/create`, { method });
  }

  resolveDid(did: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/did/resolve/${did}`);
  }

  getSupportedMethods(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/did/methods`);
  }

  // OID4VCI
  createCredentialOffer(request: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/oid4vci/credential-offer`, request);
  }

  getIssuerMetadata(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/oid4vci/metadata`);
  }

  // OID4VP
  createPresentationRequest(scenario: string): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/oid4vp/request`, { scenario });
  }

  submitPresentation(response: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/oid4vp/response`, response);
  }

  getVerificationScenarios(): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/oid4vp/scenarios`);
  }
}
