import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CredentialService } from 'src/app/core/services/credential.service';

@Component({
  selector: 'app-issue-credential',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="container mt-4">
      <h2>Issue Verifiable Credential</h2>
      <p class="text-muted">Create and issue an SD-JWT Verifiable Credential</p>
      <a routerLink="/credentials" class="btn btn-sm btn-outline-secondary mb-3">Back to Dashboard</a>

      <div *ngIf="error" class="alert alert-danger">{{ error }}</div>
      <div *ngIf="success" class="alert alert-success">{{ success }}</div>

      <div class="card">
        <div class="card-body">
          <form (ngSubmit)="issue()">
            <div class="row mb-3">
              <div class="col-md-6">
                <label class="form-label">Credential Type</label>
                <select class="form-select" [(ngModel)]="credentialType" name="credentialType"
                        (change)="onTypeChange()">
                  <option value="I20Credential">I-20 Credential</option>
                  <option value="FinancialSupportCredential">Financial Support Credential</option>
                  <option value="PassportCredential">Passport Credential</option>
                  <option value="VisaCredential">Visa Credential</option>
                  <option value="DS2019Credential">DS-2019 Credential</option>
                  <option value="I94Credential">I-94 Credential</option>
                </select>
              </div>
              <div class="col-md-3">
                <label class="form-label">Issuer DID</label>
                <input type="text" class="form-control" [(ngModel)]="issuerDid" name="issuerDid"
                       placeholder="did:key:...">
              </div>
              <div class="col-md-3">
                <label class="form-label">Subject DID</label>
                <input type="text" class="form-control" [(ngModel)]="subjectDid" name="subjectDid"
                       placeholder="did:key:...">
              </div>
            </div>

            <h5>Claims</h5>
            <div class="row mb-2" *ngFor="let claim of claimFields">
              <div class="col-md-4">
                <label class="form-label">{{ claim.label }}</label>
              </div>
              <div class="col-md-8">
                <input type="text" class="form-control form-control-sm"
                       [(ngModel)]="claims[claim.key]" [name]="claim.key"
                       [placeholder]="claim.placeholder || ''">
              </div>
            </div>

            <div class="mb-3">
              <label class="form-label">Validity (days)</label>
              <input type="number" class="form-control" [(ngModel)]="validityDays" name="validityDays"
                     style="width: 150px">
            </div>

            <button type="submit" class="btn btn-primary" [disabled]="issuing">
              {{ issuing ? 'Issuing...' : 'Issue Credential' }}
            </button>
          </form>
        </div>
      </div>

      <div *ngIf="issuedCredential" class="card mt-3">
        <div class="card-header bg-success text-white">Credential Issued</div>
        <div class="card-body">
          <p><strong>ID:</strong> {{ issuedCredential.credentialId }}</p>
          <p><strong>Format:</strong> {{ issuedCredential.format }}</p>
          <div class="mb-2">
            <label class="form-label"><strong>SD-JWT:</strong></label>
            <textarea class="form-control font-monospace" rows="4" readonly
                      [value]="issuedCredential.serializedCredential"></textarea>
          </div>
        </div>
      </div>
    </div>
  `
})
export class IssueCredentialComponent {
  credentialType = 'I20Credential';
  issuerDid = 'did:key:z6MkIssuer';
  subjectDid = 'did:key:z6MkStudent';
  validityDays = 365;
  claims: Record<string, any> = {};
  claimFields: { key: string; label: string; placeholder?: string }[] = [];

  issuing = false;
  error = '';
  success = '';
  issuedCredential: any = null;

  constructor(private credentialService: CredentialService, private router: Router) {
    this.onTypeChange();
  }

  onTypeChange() {
    this.claims = {};
    if (this.credentialType === 'I20Credential') {
      this.claimFields = [
        { key: 'sevisId', label: 'SEVIS ID', placeholder: 'N0001234567' },
        { key: 'studentName', label: 'Student Name', placeholder: 'John Doe' },
        { key: 'programStatus', label: 'Program Status', placeholder: 'Active' },
        { key: 'educationLevel', label: 'Education Level', placeholder: "Master's" },
        { key: 'primaryMajor', label: 'Primary Major', placeholder: 'Computer Science' },
        { key: 'programStartDate', label: 'Program Start', placeholder: '2024-08-15' },
        { key: 'programEndDate', label: 'Program End', placeholder: '2026-05-15' },
        { key: 'institutionName', label: 'Institution', placeholder: 'Test University' }
      ];
    } else if (this.credentialType === 'FinancialSupportCredential') {
      this.claimFields = [
        { key: 'sevisId', label: 'SEVIS ID', placeholder: 'N0001234567' },
        { key: 'studentName', label: 'Student Name', placeholder: 'John Doe' },
        { key: 'academicTerm', label: 'Academic Term', placeholder: 'Fall 2024' },
        { key: 'totalExpenses', label: 'Total Expenses ($)', placeholder: '45000' },
        { key: 'totalFunding', label: 'Total Funding ($)', placeholder: '50000' },
        { key: 'tuition', label: 'Tuition ($)', placeholder: '25000' },
        { key: 'livingExpenses', label: 'Living Expenses ($)', placeholder: '15000' },
        { key: 'personalFunds', label: 'Personal Funds ($)', placeholder: '10000' },
        { key: 'schoolFunds', label: 'School Funds ($)', placeholder: '30000' }
      ];
    } else if (this.credentialType === 'PassportCredential') {
      this.claimFields = [
        { key: 'holderName', label: 'Holder Name', placeholder: 'Jane Doe' },
        { key: 'nationality', label: 'Nationality', placeholder: 'United States' },
        { key: 'issuingState', label: 'Issuing State', placeholder: 'USA' },
        { key: 'documentNumber', label: 'Document Number', placeholder: '123456789' },
        { key: 'dateOfBirth', label: 'Date of Birth', placeholder: '1995-03-15' },
        { key: 'expirationDate', label: 'Expiration Date', placeholder: '2035-03-14' },
        { key: 'sex', label: 'Sex', placeholder: 'F' }
      ];
    } else if (this.credentialType === 'VisaCredential') {
      this.claimFields = [
        { key: 'holderName', label: 'Holder Name', placeholder: 'Jane Doe' },
        { key: 'visaType', label: 'Visa Type/Classification', placeholder: 'F-1' },
        { key: 'issuingPost', label: 'Issuing Post', placeholder: 'London' },
        { key: 'issueDate', label: 'Issue Date', placeholder: '2024-06-01' },
        { key: 'expirationDate', label: 'Expiration Date', placeholder: '2028-06-01' },
        { key: 'stampNumber', label: 'Stamp Number', placeholder: '20241234567' },
        { key: 'controlNumber', label: 'Control Number', placeholder: '20241234567890' }
      ];
    } else if (this.credentialType === 'DS2019Credential') {
      this.claimFields = [
        { key: 'sevisId', label: 'SEVIS ID', placeholder: 'N0001234567' },
        { key: 'participantName', label: 'Participant Name', placeholder: 'Jane Doe' },
        { key: 'programSponsor', label: 'Program Sponsor', placeholder: 'University Exchange Program' },
        { key: 'programNumber', label: 'Program Number', placeholder: 'P-1-00001' },
        { key: 'categoryCode', label: 'Category Code', placeholder: '1A' },
        { key: 'programStartDate', label: 'Program Start', placeholder: '2024-08-15' },
        { key: 'programEndDate', label: 'Program End', placeholder: '2025-08-14' }
      ];
    } else if (this.credentialType === 'I94Credential') {
      this.claimFields = [
        { key: 'holderName', label: 'Holder Name', placeholder: 'Jane Doe' },
        { key: 'i94Number', label: 'I-94 Number', placeholder: '12345678901' },
        { key: 'classOfAdmission', label: 'Class of Admission', placeholder: 'F-1' },
        { key: 'admissionDate', label: 'Admission Date', placeholder: '2024-08-10' },
        { key: 'admittedUntil', label: 'Admitted Until', placeholder: 'D/S' },
        { key: 'portOfEntry', label: 'Port of Entry', placeholder: 'JFK' }
      ];
    }
  }

  issue() {
    this.issuing = true;
    this.error = '';
    this.success = '';
    this.issuedCredential = null;

    this.credentialService.issueCredential({
      issuerDid: this.issuerDid,
      subjectDid: this.subjectDid,
      credentialType: this.credentialType,
      claims: this.claims,
      validityDays: this.validityDays
    }).subscribe({
      next: (result) => {
        this.issuedCredential = result;
        this.success = 'Credential issued successfully!';
        this.issuing = false;
      },
      error: (err: any) => {
        this.error = err.error?.error || 'Issuance failed';
        this.issuing = false;
      }
    });
  }
}
