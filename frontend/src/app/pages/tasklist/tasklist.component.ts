import { Component, OnInit } from "@angular/core";

@Component({
  selector: "tasklist",
  templateUrl: "./tasklist.component.html",
  styleUrls: ["./tasklist.component.scss"]
})
export class TaskListComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
    console.log('herpderp');
  }
}
