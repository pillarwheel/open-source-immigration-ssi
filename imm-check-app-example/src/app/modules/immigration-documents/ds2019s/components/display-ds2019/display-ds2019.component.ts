import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { of } from 'rxjs';
import { ds2019 } from '../../../../../shared/models/ds2019';

@Component({
  selector: 'app-display-ds2019',
  templateUrl: './display-ds2019.component.html',
  styleUrls: ['./display-ds2019.component.css']
})

export class DisplayDs2019Component implements OnInit {
  public ds2019s: ds2019[];
  private ds2019sRoute = 'https://localhost:7272/api/database';
  
  @Input() ds2019: ds2019;
  @Output() ds2019Deleted: EventEmitter<number> = new EventEmitter();
  @Output() ds2019Viewed: EventEmitter<string> = new EventEmitter();

  constructor(private http: HttpClient) { }

  getDs2019s() {
    this.http.get<ds2019[]>(this.ds2019sRoute).subscribe(ds2019s => {
      this.ds2019s = ds2019s;
      console.log('Ds2019s: ', this.ds2019s);
    });
  }

  ngOnInit(): void {
    this.getDs2019s();
  }

/*
  onDs2019Viewed(url) {
    window.open(url);
  }
*/

  onClickDelete() {
    alert("Delete!");
    this.ds2019Deleted.emit(this.ds2019.idnumber);
  }

}
