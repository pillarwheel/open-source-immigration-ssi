import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
//import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ds2019 } from 'src/app/shared/models/ds2019';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { MdbFormsModule } from 'mdb-angular-ui-kit/forms';

@Component({
  selector: 'app-display-ds2019',
  templateUrl: './display-ds2019.component.html',
  styleUrls: ['./display-ds2019.component.css']
})
export class DisplayDs2019Component implements OnInit {
  @Input() ds2019: ds2019;
  @Output() ds2019Deleted: EventEmitter<number> = new EventEmitter();
  @Output() ds2019Viewed: EventEmitter<string> = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }

  onClickDelete() {
    this.ds2019Deleted.emit(this.ds2019.id);
  }

  onClickView() {
    this.ds2019Viewed.emit(this.ds2019.ipfsCID);
  }

  onClickSubmit(result: any) {
    console.log("You have entered : " + result.username); 
  }
//ipfs://QmQqzMTavQgT4f4T5v6PWBp7XNKtoPmC9jvn12WPT3gkSE
//https://ipfs.io/ipfs/QmQqzMTavQgT4f4T5v6PWBp7XNKtoPmC9jvn12WPT3gkSE
//https://ipfs.io/ipfs/QmbD78XdKZXFjiP8QUvwtRkgmp4xa4CGRuXLj5BGyV7C4A?filename=passport-with_mrz-example_a.jpg
}
