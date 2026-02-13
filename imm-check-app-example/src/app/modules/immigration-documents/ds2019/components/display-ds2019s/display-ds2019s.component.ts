import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DocumentService } from 'src/app/core/services/document.service';
import { ds2019 } from 'src/app/shared/models/ds2019';

@Component({
  selector: 'app-display-ds2019s',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './display-ds2019s.component.html',
  styleUrls: ['./display-ds2019s.component.css']
})
export class DisplayDs2019sComponent implements OnInit {
  public ds2019s: ds2019[];

  constructor(private documentService: DocumentService) { }

  ngOnInit(): void {
    this.getDs2019s();
  }

  getDs2019s() {
    this.documentService.getDS2019s().subscribe(ds2019s => {
      this.ds2019s = ds2019s;
    });
  }

  onDs2019Deleted(ds2019Id: number) {
    const index = this.ds2019s.findIndex(d => d.id === ds2019Id);
    if (index !== -1) {
      this.ds2019s.splice(index, 1);
    }
  }

  onDs2019Viewed(CID: string) {
    window.open("https://ipfs.io/ipfs/" + CID);
  }
}
