import { Injectable } from '@angular/core';
// update 14/07
//  import moment from "moment-timezone";
import * as moment from 'moment';
@Injectable()
export class DataFormatService {

  constructor() { }

  // Tiền
  moneyFormat(value: Number | number | string) {
    return value ? Math.round(Number(value)).toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,') : '0';
  }

  floatMoneyFormat(value: any) {
    return value ? Number(value).toFixed(2).toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,') : '0.00';
  }

  floatMoneyFiveFormart(value: any) {
      return value ? Number(value).toFixed(5).toString() : '0.00000';
  }

  // Số
  numberFormat(value: string | number) {
    if (value === 0) {
      return '0';
    }
    // @ts-ignore
    // return value ? parseFloat(Math.round((+value) * 100) / 100).toFixed(2) : '';
    return value ? parseFloat(value).toString() : '';
  }

  numberValidate(value: string | number) {
    const NUMBER_REGEX = /^(-?\d+\.\d+)$|^(-?\d+)$|null|undefined/;
    //const NUMBER_REGEX = /^(0|[1-9]\d*)?(\.\d+)?(?<=\d)$/;
    return NUMBER_REGEX.test(value?.toString());
  }

  // Ngày
  dateFormat(val: string | moment.Moment | Date) {
    // if (val == '0001-01-01T00:00:00' || val == '9999-12-31T23:59:59.9999999') return '';
    return val ? moment(val).tz(this.getTimeZone()).local().format('DD/MM/YYYY') : '';
  }

  monthFormat(val: string | number) {
    return val ? moment(val).format('MM-YYYY') : ''
  }

  timeFormat(val: string | moment.Moment | Date) {
    return val ? moment(val).format('HH:mm') : ''
  }

  getTimeZone() {
    const localeTimezone = abp.localization.currentLanguage;
    if (localeTimezone.name === 'vi') return 'Asia/Ho_Chi_Minh';
    if (localeTimezone.name === 'en') return 'America/New_York';
    if (localeTimezone.name === 'es-MX') return 'Mexico';
    if (localeTimezone.name === 'es') return 'Spain';
    return '';
  }

  // Ngày giờ
  // dateTimeFormat(val: any) {
  //   // if (val == '0001-01-01T00:00:00' || val == '9999-12-31T23:59:59.9999999') return '';
  //   return val ? moment(val).tz(this.getTimeZone()).local().format('DD/MM/YYYY, HH:mm') : '';
  // }

  // Biển số xe
  registerNoValidate(registerNo: string) {
    const REGISTER_NO_REGEX = /^\d{2}\D{1}[-. ]?\d{4}[\d{1}]?$/g;
    return REGISTER_NO_REGEX.test(registerNo);
  }

  // Số điện thoại
  phoneNumberValidate(phoneNumber: string) {
    const PHONE_NUMBER_REGEX = /(0|[+]([0-9]{2})){1}[ ]?[0-9]{2}([-. ]?[0-9]){7}|((([0-9]{3}[- ]){2}[0-9]{4})|((0|[+][0-9]{2}[- ]?)(3|7|8|9|1)([0-9]{8}))|(^[\+]?[(][\+]??[0-9]{2}[)]?([- ]?[0-9]{2}){2}([- ]?[0-9]{3}){2}))$/gm;
    return !phoneNumber || PHONE_NUMBER_REGEX.test(phoneNumber);
  }

  // Validate Phone number
  phoneNumberCheckValidate(phoneNumber?: string) {
      const PHONE_NUMBER_VALIDATE = /(^\(?([0-9]{4})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$)|(^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$)|(^\(?([0-9]{3}|[0-9]{4})\)?([0-9]{3}|[0-9]{4})?([0-9]{3}|[0-9]{4})$)/gm;
      return !phoneNumber || PHONE_NUMBER_VALIDATE.test(phoneNumber);
  }
  // Mã màu
  colorValidate(color: string) {
    const COLOR_REGEX = /(?:#|0x)(?:[a-f0-9]{3}|[a-f0-9]{6})\b|(?:rgb|hsl)a?\([^\)]*\)/ig;
    return COLOR_REGEX.test(color);
  }

  // Không được là số âm
  notNegativeNumberValidate(params: number | string) {
    return !(params !== '' && (params && Number(params) < 0));
  }

  // Số nguyên dương
  positiveNumberValidate(params: string | number) {
    const NUMBER_REG = /^(?!(?:^[-+]?[0.]+(?:[Ee]|$)))(?!(?:^-))(?:(?:[+-]?)(?=[0123456789.])(?:(?:(?:[0123456789]+)(?:(?:[.])(?:[0123456789]*))?|(?:(?:[.])(?:[0123456789]+))))(?:(?:[Ee])(?:(?:[+-]?)(?:[0123456789]+))|))$/
    //const NUMBER_REG = /^\d*[1-9]+\d*$/;
    // return !(params.value !== '' && ((params.value && Number(params.value) <= 0) || !NUMBER_REG.test(params.value)));
    return NUMBER_REG.test(params.toString());
  }

  // Số nguyên không âm
  notNegativeIntNumberValidate(params: any) {
    const NUMBER_REG = /^\d+$/g;
    return NUMBER_REG.test(params);
  }

  // Số nguyên
  intNumberValidate(params: string) {
    const NUMBER_REG = /^([+-]?[1-9]\d*|0)$/;
    return !(params !== '' && !NUMBER_REG.test(params));
  }

  emailValidate(email: string) {
    const NUMBER_REG = /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/g;
    return NUMBER_REG.test(email);
  }

  /* For Sale */
  // Format input trong Cell Grid sang dạng DateTime để truyền xuống database
  formatInputToDateTime(input: string | Date) {
    const date = new Date(input);
    return date;
  }

  // format từ số giây sang giờ
  formatHoursSecond(seconds: number) {
    if (seconds && seconds > 0) {
      const hours = Math.floor(seconds / 3600) < 10 ? `0${Math.floor(seconds / 3600)}` : Math.floor(seconds / 3600);
      const minutes = Math.floor((seconds % 3600) / 60) < 10 ? `0${Math.floor((seconds % 3600) / 60)}` : Math.floor((seconds % 3600) / 60);
      // const second = seconds % 60 < 10 ? `0${seconds % 60}` : seconds % 60;
      return `${hours}:${minutes}`;
    } else if (seconds === 0) {
      return `00:00`;
    } else if (!seconds) {
      return '';
    }
  }

  //convert từ giờ nhập vào sang số giây
  convertTimeToSeconds(time: string): number {
    return time.split(':').reverse().reduce((prev, curr, i) => prev + +curr * Math.pow(60, i), 0);
  }

  //format Date for sale
  formatDateForSale(date: any) {
    const isFirefox = /Firefox[\/\s](\d+\.\d+)/.test(navigator.userAgent);
    if (date) {
      let convertDate;
      if (typeof date === 'string' && date.length === 1) {
        return date;
      }

      if (isFirefox && typeof date === 'string') {
        const dateArr = date.split('-');
        date = `${dateArr[1]} ${dateArr[0]}, ${dateArr[2]}`;
      }
      convertDate = new Date(date);
      const displayDate = convertDate.getDate() < 10 ? `0${convertDate.getDate()}` : convertDate.getDate();
      const formattedMonth = convertDate.getMonth() < 9 ? `0${convertDate.getMonth() + 1}` : convertDate.getMonth() + 1;
      const displayMonth = moment(formattedMonth, 'MM').format('MMM');

      return convertDate ? `${displayDate}-${displayMonth}-${convertDate.getFullYear()}` : '';
    }
    return '';
  }

  formatDisplayValue(val: any, hideDecimal?: boolean) {
    val = hideDecimal ? val : this.numberFormat(val);
    if (val) {
      if (typeof val === 'string') {
        let num = val.trim().replace(/,([0-9]{3})/g, '$1');
        if ((+num).toString() === num) {
          return (+num).toLocaleString();
        }

        num = val.trim().replace(/,/g, '');
        const NUMBER_REGEX = /^([0-9]*)$/g;
        if (NUMBER_REGEX.test(num)) {
          return (+num).toLocaleString();
        }
        return val;
      } else {
        const num = val.toString().replace(/,/g, '');
        return (+num) ? (+num).toLocaleString() : val;
      }
    }
    return '';
  }

  formatMoney(val: number | string) {
    if (val) {
      if (typeof val === 'string') {
        let num = val.trim().replace(/\,([0-9]{3})/g, '$1');

        num = val.trim().replace(/\,/g, '');
        const NUMBER_REGEX = /^([0-9]*)$/g;
        if (NUMBER_REGEX.test(num)) {
          return parseFloat(num).toLocaleString('en-US');
        }
        return val;
      } else {
        const num = val.toString().replace(/\,/g, '');
        return parseFloat(num) ? parseFloat(num).toLocaleString('en-US') : val;
      }
    }
    return 0;
  }
}
