import { Injectable } from '@angular/core';

@Injectable()
export class CommonService {
  isEmpty(value) {
    return (!value || (value.length === 0) || (this.isPlainObject(value) && Object.keys(value).length === 0));
  }

  isPlainObject(obj) {
    return Object.prototype.toString.call(obj) === '[object Object]';
  }

  swapArrayItems(arrayObj: Array<any>, i, j) {
    if (!arrayObj) {
      return arrayObj;
    }
    if (i < 0 || i >= arrayObj.length || j < 0 || j >= arrayObj.length) {
      return arrayObj;
    }

    const temp = arrayObj[i];
    arrayObj[i] = arrayObj[j];
    arrayObj[j] = temp;
    return arrayObj;
  }

  sumObjectByField(arrayObj: Array<any>, field) {
    if (this.isEmpty(arrayObj)) {
      return 0;
    }
    let sum = 0;
    for (let i = 0, length = arrayObj.length; i < length; i++) {
      sum += (Number(arrayObj[i][field]) || 0);
    }
    return sum;
  }

  sumObjectByMultipleField(arrayObj: Array<any>, fieldList) {
    if (this.isEmpty(arrayObj)) {
      return 0;
    }
    let sum = 0;
    let multi;
    for (let i = 0, length = arrayObj.length; i < length; i++) {
      multi = 1;
      for (let j = 0, len = fieldList.length; j < len; j++) {
        multi *= Number(arrayObj[i][fieldList[j]]);
      }
      sum += multi;
    }
    return sum;
  }

  compare(obj1, obj2) {
    if (this.isEmpty(obj1) && this.isEmpty(obj2)) {
      return true;
    }
    if (this.isEmpty(obj1) || this.isEmpty(obj2)) {
      return false;
    }
    return JSON.stringify(obj1) === JSON.stringify(obj2);
  }

  isObjectInListByKey(arrayObj: Array<any>, obj, key) {
    if (!arrayObj) {
      return false;
    }
    for (let i = 0, length = arrayObj.length; i < length; i++) {
      if (arrayObj[i][key] === obj[key]) {
        return true;
      }
    }
    return false;
  }

  removeDuplicateArrOfObj(arrayObj: Array<any>, key1, key2) {
    return arrayObj.filter((object, index, self) => self.findIndex(item => item[key1] === object[key1] && item[key2] === object[key2]) === index);
  }

  fromDate(timestamp) {
    return (new Date(new Date(timestamp).setHours(0, 0, 0, 0)).getTime());
  }

  toDate(timestamp) {
    return (new Date(new Date(timestamp).setHours(23, 59, 59, 999)).getTime());
  }

  checkInt(val) {
    return val % 1 === 0;
  }

  getStartOfDay() {
    return new Date().setHours(0, 0, 0, 0);
  }

  getEndOfDay() {
    return new Date().setHours(23, 59, 59, 999);
  }

  isNullOrEmpty(item) {
    if (item === null || item === '') {
      return true;
    }
    return false;
  }

  convertStringMoneyToInt(val) {
    if (val) {
      return (typeof val === 'string') ? val.replace(/,/g, '') : val;
    }
    return null;
  }
}