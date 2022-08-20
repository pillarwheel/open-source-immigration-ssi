import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { of } from 'rxjs';
import { ds2019 } from 'src/app/shared/models/ds2019';

@Component({
  selector: 'app-display-ds2019s',
  templateUrl: './display-ds2019s.component.html',
  styleUrls: ['./display-ds2019s.component.css']
})
export class DisplayDs2019sComponent implements OnInit {
  private ds2019sRoute = 'https://localhost:7272/api/database';
  public ds2019s: ds2019[];

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.getDs2019s();
  }

  getDs2019s() {
    this.http.get<ds2019[]>(this.ds2019sRoute).subscribe(ds2019s => {
        this.ds2019s = ds2019s;
        console.log('DS2019s: ', this.ds2019s);
    });
  }
  
  onDs2019Deleted(ds2019Id: number) {
    let ds2019Index = 0;
    for (let ds2019 of this.ds2019s) {
      if(ds2019.id === ds2019Id) {
        this.ds2019s.splice(ds2019Index, 1);
        break;
      }
      ds2019Index++;
    }
  }

  onDs2019Viewed(CID: string) {
    window.open("https://ipfs.io/ipfs/" + CID);
  }
//ipfs://QmQqzMTavQgT4f4T5v6PWBp7XNKtoPmC9jvn12WPT3gkSE
//https://ipfs.io/ipfs/QmQqzMTavQgT4f4T5v6PWBp7XNKtoPmC9jvn12WPT3gkSE
//PP Example: QmbD78XdKZXFjiP8QUvwtRkgmp4xa4CGRuXLj5BGyV7C4A
//https://ipfs.io/ipfs/QmbD78XdKZXFjiP8QUvwtRkgmp4xa4CGRuXLj5BGyV7C4A?filename=passport-with_mrz-example_a.jpg
}
