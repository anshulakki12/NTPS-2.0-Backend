import { Component } from '@angular/core';
import { SharedModule } from '../shared.module';

@Component({
  selector: 'app-ntps-header-navigation',
  imports: [SharedModule],
  templateUrl: './ntps-header-navigation.html',
  styleUrl: './ntps-header-navigation.css'
})
export class NtpsHeaderNavigation {

  ngOnInit(): void {
  }

fontSizeLevel = 1; // 1 = normal, 0 = small, 2 = large

changeFontSize(size: 'small' | 'normal' | 'large') {
  if (size === 'small') {
    this.fontSizeLevel = 0;
    document.documentElement.style.fontSize = '14px';
  } else if (size === 'normal') {
    this.fontSizeLevel = 1;
    document.documentElement.style.fontSize = '16px';
  } else if (size === 'large') {
    this.fontSizeLevel = 2;
    document.documentElement.style.fontSize = '18px';
  }
}

translateSite(event: Event) {
  const select = event.target as HTMLSelectElement;
  const lang = select.value;

  this.waitForGoogleCombo(() => {
    const combo = document.querySelector('.goog-te-combo') as HTMLSelectElement;
    if (combo) {
      combo.value = lang;
      combo.dispatchEvent(new Event('change'));
    }
  });
}

waitForGoogleCombo(callback: () => void) {
  const checkExist = setInterval(() => {
    const combo = document.querySelector('.goog-te-combo');
    if (combo) {
      clearInterval(checkExist);
      callback();
    }
  }, 500);
}





  private getLangName(langCode: string): string {
    const map: any = {
      en: 'English',
      hi: 'Hindi',
      ta: 'Tamil',
      bn: 'Bengali',
      gu: 'Gujarati',
      ml: 'Malayalam',
      te: 'Telugu',
      pa: 'Punjabi',
      mr: 'Marathi'
    };
    return map[langCode] || 'English';
  }
}
