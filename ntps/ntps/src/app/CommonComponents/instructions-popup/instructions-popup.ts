import { CommonModule } from '@angular/common';
import { Component, Inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-instructions-popup',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule,MatDividerModule],
  templateUrl: './instructions-popup.html',
  styleUrl: './instructions-popup.css'
})
export class InstructionsPopup {
constructor(
    public dialogRef: MatDialogRef<InstructionsPopup>,
    @Inject(MAT_DIALOG_DATA) public data: { title: string; instructions: string[] }
  ) {}

  closeDialog(): void {
    this.dialogRef.close();
  }
}
