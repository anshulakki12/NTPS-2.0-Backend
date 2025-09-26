import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatDialogModule } from '@angular/material/dialog';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatToolbar } from '@angular/material/toolbar';
import { MatMenu, MatMenuTrigger } from '@angular/material/menu';
import { MatExpansionModule } from '@angular/material/expansion';
@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
     MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatDividerModule,
    MatDialogModule,
    MatProgressBarModule,
    FormsModule, 
    MatToolbar,
    MatMenu,
    MatMenuTrigger,
    MatExpansionModule

  ],
  exports: [
    CommonModule,
    ReactiveFormsModule,
       MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    MatDividerModule,
    MatDialogModule,
    MatProgressBarModule,
    FormsModule, 
    MatToolbar,
    MatMenu,
    MatMenuTrigger,
    MatExpansionModule
  ]
})
export class SharedModule {}
