import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';

import { ds2019 } from '../../../shared/models/ds2019';

@Component({
  selector: 'app-display-ds2019',
  templateUrl: './ds2019.component.html',
  styleUrls: ['./ds2019.component.css']
})
export class Ds2019Component implements OnInit {
  ds2019s: ds2019[];
  @Input() ds2019: ds2019;
  @Output() ds2019Deleted: EventEmitter<number> = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }

  onClickDelete() {
    alert("Delete!");
    this.ds2019Deleted.emit(this.ds2019.idnumber);
  }

}
