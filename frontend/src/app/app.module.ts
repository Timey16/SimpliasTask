
import { NgModule } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientJsonpModule, HttpClientModule } from '@angular/common/http';
import en from '@angular/common/locales/en';

import { NZ_ICONS } from 'ng-zorro-antd/icon';
import { NZ_I18N, en_US } from 'ng-zorro-antd/i18n';
import { IconDefinition } from '@ant-design/icons-angular';
import * as AllIcons from '@ant-design/icons-angular/icons';

import { DemoNgZorroAntdModule } from './ng-zorro-antd.module';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { TaskListComponent } from './pages/tasklist/tasklist.component';
import { AppComponent } from './app.component';
import { RouterModule } from '@angular/router';
import { RoutingModule } from './routing.module';
import { TaskModalComponent } from './pages/taskModal/taskModal.component';

registerLocaleData(en);

const antDesignIcons = AllIcons as {
  [key: string]: IconDefinition;
};
const icons: IconDefinition[] = Object.keys(antDesignIcons).map(key => antDesignIcons[key])

@NgModule({
  imports: [
    BrowserModule, FormsModule,
    HttpClientModule,
    HttpClientJsonpModule,
    ReactiveFormsModule,
    DemoNgZorroAntdModule,
    BrowserAnimationsModule,
    ScrollingModule,
    DragDropModule,
    RouterModule,
    RoutingModule
  ],
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    TaskListComponent,
    TaskModalComponent
  ],
  bootstrap: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    TaskListComponent,
    TaskModalComponent
  ],
  providers: [{ provide: NZ_I18N, useValue: en_US }, { provide: NZ_ICONS, useValue: icons }]
})
export class AppModule { }
