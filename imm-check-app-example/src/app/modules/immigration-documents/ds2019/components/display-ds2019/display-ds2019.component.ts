import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ds2019 } from 'src/app/shared/models/ds2019';

@Component({
  selector: 'app-display-ds2019',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './display-ds2019.component.html',
  styleUrls: ['./display-ds2019.component.css']
})
export class DisplayDs2019Component implements OnInit {
  @Input() ds2019: ds2019;
  @Output() ds2019Deleted: EventEmitter<number> = new EventEmitter();
  @Output() ds2019Viewed: EventEmitter<string> = new EventEmitter();

  constructor() { }

  ngOnInit(): void { }

  onClickDelete() {
    this.ds2019Deleted.emit(this.ds2019.id);
  }

  onClickView() {
    this.ds2019Viewed.emit(this.ds2019.ipfsCID);
  }

  onClickSubmit(result: any) {
    console.log("You have entered : " + result.username);
  }
}
