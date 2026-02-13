# Regulatory Compliance & Standards

This document details the regulatory landscape, compliance considerations, and standards alignment for the ImmCheck immigration credential system.

---

## 1. U.S. Immigration Technology Landscape

### SEVP/SEVIS

The **Student and Exchange Visitor Program (SEVP)**, administered by U.S. Immigration and Customs Enforcement (ICE), manages over 1.1 million active F, J, and M visa holders through the **Student and Exchange Visitor Information System (SEVIS)** — the federal database tracking all international students and exchange visitors in the United States.

**Designated School Officials (DSOs)** at SEVP-certified institutions are responsible for:
- Creating and maintaining SEVIS records for international students
- Generating I-20 forms (Certificate of Eligibility for F-1 status) through SEVIS
- Reporting enrollment status, program extensions, and employment authorization
- Verifying financial support documentation

SEVIS currently operates as a centralized batch-processing system (Release 6.80), with institutions submitting updates through a web portal or batch file upload. There is no standards-based API for credential exchange.

**How VCs improve this workflow:** Verifiable Credentials issued when SEVIS generates an I-20 enable instant, cryptographic verification of enrollment status — eliminating manual verification calls between institutions and reducing document fraud.

### Other Federal Systems

| System | Agency | Function |
|--------|--------|----------|
| **E-Verify** | USCIS | Employment eligibility verification |
| **SAVE** | USCIS | Systematic Alien Verification for Entitlements |
| **CBP TVS** | CBP | Traveler Verification Service (biometric) |
| **myUSCIS** | USCIS | Online immigration case management |

---

## 2. DHS SVIP Digital Credentials Program

The **DHS Science & Technology Directorate's Silicon Valley Innovation Program (SVIP)** has been actively funding blockchain and digital credential solutions for immigration since 2018, with over 60 companies receiving funding across multiple cohorts.

### Key Players

| Company | DHS Contract Focus | Notable |
|---------|-------------------|---------|
| **Danube Tech** | Digital Permanent Resident Card (Green Card) | Building VC-based PRC with USCIS |
| **MATTR** | VC issuance infrastructure for USCIS | Enterprise credential platform |
| **SpruceID** | Digital identity infrastructure | $42M raised; direct DHS contract for government identity |
| **Digital Bazaar** | VC/DID standards implementation | Core W3C VC spec contributor |
| **Transmute** | Supply chain + immigration credentials | CBP digital trade credentials |
| **Mavennet** | Cross-border trade verification | Steel import credentials with CBP |

### USCIS Standards Adoption

USCIS leadership has publicly stated commitment to standards-based digital credentials:
- USCIS and CBP are actively pursuing W3C Verifiable Credential and DID standards
- The July 2023 USCIS policy update accepts electronic signatures on immigration forms
- A November 2025 proposed rule addresses "electronic document matching" for immigration benefit applications
- DHS SVIP specifically funds W3C VC/DID implementations (not proprietary formats)

### Alignment with ImmCheck

This project implements the same W3C VC 2.0 and DID Core standards that DHS SVIP is funding commercial companies to build. The open-source approach provides a reference implementation that universities can deploy without vendor lock-in.

---

## 3. Regulatory Framework

### Current Legal Status

No federal law specifically authorizes or prohibits the use of blockchain-verified documents for immigration purposes. The regulatory environment is characterized by:

- **Active piloting:** DHS is the largest U.S. government funder of blockchain identity projects
- **Electronic signatures accepted:** USCIS began accepting e-signatures on forms (July 2023)
- **Electronic document matching:** Proposed rule (November 2025) would formalize electronic verification of immigration documents
- **No blockchain prohibition:** Neither INA (Immigration and Nationality Act) nor 8 CFR regulations prohibit cryptographically verified credentials
- **NIST alignment:** NIST SP 800-63-4 (July 2025) includes digital wallets in the federal identity assurance framework, providing a federal basis for VC-based identity verification

### Practical Implications

VCs do not replace official immigration documents — they provide a verifiable digital representation that can be cryptographically validated. An I-20 Verifiable Credential does not replace the SEVIS-generated I-20; it provides a tamper-evident, selectively disclosable digital version that can be verified without calling the issuing institution.

---

## 4. FERPA Compliance

The **Family Educational Rights and Privacy Act (FERPA)** governs the privacy of student education records at institutions receiving federal funding.

### ImmCheck FERPA Alignment

| FERPA Requirement | ImmCheck Architecture |
|-------------------|----------------------|
| **No unauthorized disclosure** | Off-chain architecture — no PII stored on any blockchain |
| **Student consent** | SD-JWT selective disclosure requires student to actively present claims |
| **School official exception** | DSOs issue credentials in their official SEVIS capacity |
| **Right to amend** | Off-chain mutable storage enables record correction; credentials can be revoked and reissued |
| **Institutional control** | Institutions control credential issuance; students control presentation |

### Privacy Architecture

- **No PII on-chain:** Only cryptographic hashes, credential schemas, and revocation registries are anchored on blockchain
- **Student-controlled presentation:** Students decide which claims to disclose via SD-JWT selective disclosure
- **Consent framework:** Every credential presentation is an explicit act of student consent
- **Revocation capability:** Institutions can revoke credentials when status changes (program completion, transfer, termination)

---

## 5. GDPR Considerations

For institutions with students subject to EU data protection law (EU/EEA nationals studying in the U.S., or U.S. institutions with EU campuses):

### EDPB Guidance (April 2025)

The European Data Protection Board's April 2025 guidelines on blockchain and data protection establish:

- **No blockchain exemption** from GDPR — data protection requirements apply regardless of storage mechanism
- **Pseudonymized data is personal data** — DIDs and hashed identifiers still constitute personal data under GDPR
- **Right to erasure challenges** — immutable blockchains create tension with Art. 17 right to erasure

### ImmCheck GDPR Approach

- **Crypto-shredding:** Destroying the issuer's signing key renders all credentials issued under that key cryptographically unverifiable — a practical equivalent of erasure
- **Off-chain PII:** All personally identifiable information is stored off-chain in mutable databases, enabling standard deletion procedures
- **Hash-only anchoring:** On-chain data consists only of cryptographic hashes, schema identifiers, and revocation bitstrings — not personal data in isolation
- **Selective disclosure by design:** SD-JWT minimizes data exposure to only the claims necessary for each verification context (data minimization principle, Art. 5(1)(c))

---

## 6. Standards Compliance

ImmCheck implements or aligns with the following specifications:

### W3C Standards

| Standard | Status | ImmCheck Implementation |
|----------|--------|------------------------|
| **Verifiable Credentials Data Model 2.0** | W3C Recommendation (May 2025) | Full implementation — 6 credential schemas |
| **DID Core v1.0** | W3C Recommendation | 5 DID methods + Universal Resolver |
| **Bitstring Status List v1.1** | W3C Working Draft | Per-issuer revocation lists (16,384 entries) |

### IETF Standards

| Standard | Status | ImmCheck Implementation |
|----------|--------|------------------------|
| **SD-JWT (Selective Disclosure for JWTs)** | IETF Draft | Custom implementation with Ed25519 + HS256 |
| **JSON Web Token (RFC 7519)** | Standard | JWT credential format |
| **JSON Web Signature (RFC 7515)** | Standard | EdDSA and HS256 signing |

### OpenID Standards

| Standard | Status | ImmCheck Implementation |
|----------|--------|------------------------|
| **OID4VCI** | OpenID Foundation Draft | Pre-authorized code flow |
| **OID4VP** | OpenID Foundation Draft | Presentation exchange with 5 scenarios |

### International Standards

| Standard | Scope | ImmCheck Implementation |
|----------|-------|------------------------|
| **ICAO 9303** | Machine Readable Travel Documents | PassportCredential maps MRZ fields |
| **NIST SP 800-63-4** (July 2025) | Digital Identity Guidelines | Digital wallet identity assurance alignment |
| **eIDAS 2.0** (EU, mandatory Dec 2026) | European Digital Identity Wallets | W3C VC format interoperability |

---

## 7. Privacy Architecture Principles

**No personal data is stored on any blockchain.** The system uses off-chain storage for all PII, anchoring only cryptographic hashes, credential schemas, and revocation registries on-chain.

### Data Flow

```
Student PII → Off-chain SQLite DB → SD-JWT credential (off-chain) → Student wallet
                                            |
                            Cryptographic hash → Blockchain (did:prism, did:midnight)
                            Revocation bitstring → Blockchain
                            Credential schema → Blockchain
```

### Selective Disclosure Model

SD-JWT enables students to prove specific facts without revealing underlying data:

| Verification Need | Claims Disclosed | Claims Hidden |
|-------------------|-----------------|---------------|
| Prove F-1 enrollment | programStatus, institutionName | sevisId, dates, major |
| Prove nationality | nationality, issuingState | passportNumber, DOB, MRZ |
| Prove financial sufficiency | totalFunding, totalExpenses | individual fund sources, sponsor name |
| Prove J-1 status | programSponsor, categoryCode | sevisId, participant name, dates |
| Prove admission class | classOfAdmission, admissionDate | i94Number, holderName |

### Zero-Knowledge Extensions (Midnight)

When Midnight mainnet launches (late March 2026), ZK-SNARKs will enable even stronger privacy guarantees beyond SD-JWT selective disclosure:

- **Boolean proofs:** Prove "is a valid F-1 student" without revealing any claim values
- **Range proofs:** Prove "financial support exceeds $X" without revealing the actual amount
- **Nationality proofs:** Prove nationality without revealing passport number, name, or date of birth

---

## 8. Atala PRISM → Hyperledger Identus

### Transition History

| Date | Event |
|------|-------|
| 2021 | IOG (Input Output Global) launches **Atala PRISM** as Cardano's identity solution |
| 2023 | IOG contributes Atala PRISM codebase to the **Hyperledger Foundation** under the Linux Foundation |
| April 2024 | Project promoted from Hyperledger Labs to full project status as **Hyperledger Identus** |
| October 2024 | IOG reduces Identus team; community maintenance continues under Hyperledger governance |
| 2025–present | Identus remains the primary `did:prism` infrastructure for Cardano |

### Current Status

Hyperledger Identus provides:
- **Cloud Agent** — REST API for DID lifecycle management (create, publish, resolve)
- **Cardano anchoring** — DID operations recorded on Cardano blockchain (preprod and mainnet)
- **W3C compliance** — DID Core and VC Data Model support
- **Open source** — Apache 2.0 license under Hyperledger Foundation governance

### Relationship to Midnight

Midnight is a separate blockchain (Cardano partner chain) focused on privacy through ZK-SNARKs. While Identus handles `did:prism` on Cardano's main chain, `did:midnight` operates on the Midnight chain. Both are IOG projects, but they serve different purposes: Identus for identity anchoring, Midnight for privacy-preserving computation.

---

## 9. Midnight & Zero-Knowledge Proofs

### What is Midnight?

Midnight is IOG's privacy-focused Cardano partner chain that uses **ZK-SNARKs** (Zero-Knowledge Succinct Non-Interactive Arguments of Knowledge) for confidential smart contracts. Mainnet is planned for late March 2026.

### Beyond SD-JWT

SD-JWT selective disclosure reveals individual claim values (e.g., "nationality: US"). ZK-proofs enable proving statements about claims without revealing the values:

| Privacy Level | Technology | Example |
|---------------|-----------|---------|
| **Full disclosure** | Standard JWT | All claims visible to verifier |
| **Selective disclosure** | SD-JWT | Choose which claims to reveal; revealed claims are fully visible |
| **Zero-knowledge** | ZK-SNARKs (Midnight) | Prove facts about claims without revealing any claim values |

### Immigration Use Cases

| Scenario | SD-JWT Approach | ZK-Proof Approach |
|----------|----------------|-------------------|
| Prove enrollment | Reveal: programStatus, institutionName | Prove: "holds valid I-20 credential" (boolean) |
| Prove financial support | Reveal: totalFunding amount | Prove: "funding ≥ $X threshold" (range proof) |
| Prove nationality | Reveal: nationality claim | Prove: "holds passport from country in approved list" (set membership) |
| Prove admission status | Reveal: classOfAdmission | Prove: "admitted in valid nonimmigrant class" (boolean) |

### ImmCheck Integration

The `did:midnight` resolver is pre-built and configured for Midnight's testnet. When mainnet launches:
1. Update `Midnight:ResolverUrl` configuration to mainnet endpoint
2. ZK-proof credential presentations can be added as a new verification mode alongside SD-JWT
3. OID4VP scenarios can offer a "zero-knowledge" presentation option for maximum privacy
