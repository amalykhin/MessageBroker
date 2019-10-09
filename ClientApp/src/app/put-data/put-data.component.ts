import { Component, Inject } from '@angular/core';
import { FormBuilder } from "@angular/forms";
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-put-data',
  templateUrl: './put-data.component.html'
})
export class PutDataComponent {
  storyForm;
  http;
  baseUrl;

  constructor(http:HttpClient, @Inject('BASE_URL') baseUrl: string, private formBuilder: FormBuilder) {
    this.storyForm = this.formBuilder.group({
      title: '',
      author: '',
      tag: '',
      body: '',
      modifiedDate: new Date()
    });
    this.http = http;
    this.baseUrl = baseUrl;
  }

  onSubmit(newsData) {
    this.storyForm.reset();
    this.http.post(`${this.baseUrl}stories`, newsData)
      .subscribe();
  }
}

interface NewsStory {
  tag: string;
  title: string;
  body: string;
  author: string;
  publishedDate: Date;
  modifiedDate: Date;
}
