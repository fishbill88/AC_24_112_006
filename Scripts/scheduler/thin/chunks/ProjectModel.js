/*!
 *
 * Bryntum Scheduler Pro 5.3.7
 *
 * Copyright(c) 2023 Bryntum AB
 * https://bryntum.com/contact
 * https://bryntum.com/license
 *
 */
import { DateHelper, TimeZoneHelper, Store, Model, ObjectHelper, Events, Delayable, Base, ArrayHelper, StringHelper, Objects, AjaxHelper, LoadMaskable, Mask, AjaxStore, VersionHelper, FunctionHelper, DayTime, DomClassList, Duration, BrowserHelper } from './Editor.js';
import { StateTrackingManager } from './AvatarRendering.js';
import { GridRowModel } from './GridRowModel.js';

// @ts-nocheck
const diffSecond = (date, diff) => {
  return new Date(date.getFullYear(), date.getMonth(), date.getDate(), date.getHours(), date.getMinutes(), date.getSeconds() + diff, date.getMilliseconds());
};
/**
 * The code just copy/pasted from pre-built later.js file and made exported
 *
 * @private
 */
const later = function () {

  var later = {
    version: "1.2.0"
  };
  if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (searchElement) {

      if (this == null) {
        throw new TypeError();
      }
      var t = Object(this);
      var len = t.length >>> 0;
      if (len === 0) {
        return -1;
      }
      var n = 0;
      if (arguments.length > 1) {
        n = Number(arguments[1]);
        if (n != n) {
          n = 0;
        } else if (n != 0 && n != Infinity && n != -Infinity) {
          n = (n > 0 || -1) * Math.floor(Math.abs(n));
        }
      }
      if (n >= len) {
        return -1;
      }
      var k = n >= 0 ? n : Math.max(len - Math.abs(n), 0);
      for (; k < len; k++) {
        if (k in t && t[k] === searchElement) {
          return k;
        }
      }
      return -1;
    };
  }
  if (!String.prototype.trim) {
    String.prototype.trim = function () {
      return this.replace(/^\s+|\s+$/g, "");
    };
  }
  later.array = {};
  later.array.sort = function (arr, zeroIsLast) {
    arr.sort(function (a, b) {
      return +a - +b;
    });
    if (zeroIsLast && arr[0] === 0) {
      arr.push(arr.shift());
    }
  };
  later.array.next = function (val, values, extent) {
    var cur,
      zeroIsLargest = extent[0] !== 0,
      nextIdx = 0;
    for (var i = values.length - 1; i > -1; --i) {
      cur = values[i];
      if (cur === val) {
        return cur;
      }
      if (cur > val || cur === 0 && zeroIsLargest && extent[1] > val) {
        nextIdx = i;
        continue;
      }
      break;
    }
    return values[nextIdx];
  };
  later.array.nextInvalid = function (val, values, extent) {
    var min = extent[0],
      max = extent[1],
      len = values.length,
      zeroVal = values[len - 1] === 0 && min !== 0 ? max : 0,
      next = val,
      i = values.indexOf(val),
      start = next;
    while (next === (values[i] || zeroVal)) {
      next++;
      if (next > max) {
        next = min;
      }
      i++;
      if (i === len) {
        i = 0;
      }
      if (next === start) {
        return undefined;
      }
    }
    return next;
  };
  later.array.prev = function (val, values, extent) {
    var cur,
      len = values.length,
      zeroIsLargest = extent[0] !== 0,
      prevIdx = len - 1;
    for (var i = 0; i < len; i++) {
      cur = values[i];
      if (cur === val) {
        return cur;
      }
      if (cur < val || cur === 0 && zeroIsLargest && extent[1] < val) {
        prevIdx = i;
        continue;
      }
      break;
    }
    return values[prevIdx];
  };
  later.array.prevInvalid = function (val, values, extent) {
    var min = extent[0],
      max = extent[1],
      len = values.length,
      zeroVal = values[len - 1] === 0 && min !== 0 ? max : 0,
      next = val,
      i = values.indexOf(val),
      start = next;
    while (next === (values[i] || zeroVal)) {
      next--;
      if (next < min) {
        next = max;
      }
      i--;
      if (i === -1) {
        i = len - 1;
      }
      if (next === start) {
        return undefined;
      }
    }
    return next;
  };
  later.day = later.D = {
    name: "day",
    range: 86400,
    val: function (d) {
      return d.D || (d.D = later.date.getDate.call(d));
    },
    isValid: function (d, val) {
      return later.D.val(d) === (val || later.D.extent(d)[1]);
    },
    extent: function (d) {
      if (d.DExtent) return d.DExtent;
      var month = later.M.val(d),
        max = later.DAYS_IN_MONTH[month - 1];
      if (month === 2 && later.dy.extent(d)[1] === 366) {
        max = max + 1;
      }
      return d.DExtent = [1, max];
    },
    start: function (d) {
      return d.DStart || (d.DStart = later.date.next(later.Y.val(d), later.M.val(d), later.D.val(d)));
    },
    end: function (d) {
      return d.DEnd || (d.DEnd = later.date.prev(later.Y.val(d), later.M.val(d), later.D.val(d)));
    },
    next: function (d, val) {
      val = val > later.D.extent(d)[1] ? 1 : val;
      var month = later.date.nextRollover(d, val, later.D, later.M),
        DMax = later.D.extent(month)[1];
      val = val > DMax ? 1 : val || DMax;
      return later.date.next(later.Y.val(month), later.M.val(month), val);
    },
    prev: function (d, val) {
      var month = later.date.prevRollover(d, val, later.D, later.M),
        DMax = later.D.extent(month)[1];
      return later.date.prev(later.Y.val(month), later.M.val(month), val > DMax ? DMax : val || DMax);
    }
  };
  later.dayOfWeekCount = later.dc = {
    name: "day of week count",
    range: 604800,
    val: function (d) {
      return d.dc || (d.dc = Math.floor((later.D.val(d) - 1) / 7) + 1);
    },
    isValid: function (d, val) {
      return later.dc.val(d) === val || val === 0 && later.D.val(d) > later.D.extent(d)[1] - 7;
    },
    extent: function (d) {
      return d.dcExtent || (d.dcExtent = [1, Math.ceil(later.D.extent(d)[1] / 7)]);
    },
    start: function (d) {
      return d.dcStart || (d.dcStart = later.date.next(later.Y.val(d), later.M.val(d), Math.max(1, (later.dc.val(d) - 1) * 7 + 1 || 1)));
    },
    end: function (d) {
      return d.dcEnd || (d.dcEnd = later.date.prev(later.Y.val(d), later.M.val(d), Math.min(later.dc.val(d) * 7, later.D.extent(d)[1])));
    },
    next: function (d, val) {
      val = val > later.dc.extent(d)[1] ? 1 : val;
      var month = later.date.nextRollover(d, val, later.dc, later.M),
        dcMax = later.dc.extent(month)[1];
      val = val > dcMax ? 1 : val;
      var next = later.date.next(later.Y.val(month), later.M.val(month), val === 0 ? later.D.extent(month)[1] - 6 : 1 + 7 * (val - 1));
      if (next.getTime() <= d.getTime()) {
        month = later.M.next(d, later.M.val(d) + 1);
        return later.date.next(later.Y.val(month), later.M.val(month), val === 0 ? later.D.extent(month)[1] - 6 : 1 + 7 * (val - 1));
      }
      return next;
    },
    prev: function (d, val) {
      var month = later.date.prevRollover(d, val, later.dc, later.M),
        dcMax = later.dc.extent(month)[1];
      val = val > dcMax ? dcMax : val || dcMax;
      return later.dc.end(later.date.prev(later.Y.val(month), later.M.val(month), 1 + 7 * (val - 1)));
    }
  };
  later.dayOfWeek = later.dw = later.d = {
    name: "day of week",
    range: 86400,
    val: function (d) {
      return d.dw || (d.dw = later.date.getDay.call(d) + 1);
    },
    isValid: function (d, val) {
      return later.dw.val(d) === (val || 7);
    },
    extent: function () {
      return [1, 7];
    },
    start: function (d) {
      return later.D.start(d);
    },
    end: function (d) {
      return later.D.end(d);
    },
    next: function (d, val) {
      val = val > 7 ? 1 : val || 7;
      return later.date.next(later.Y.val(d), later.M.val(d), later.D.val(d) + (val - later.dw.val(d)) + (val <= later.dw.val(d) ? 7 : 0));
    },
    prev: function (d, val) {
      val = val > 7 ? 7 : val || 7;
      return later.date.prev(later.Y.val(d), later.M.val(d), later.D.val(d) + (val - later.dw.val(d)) + (val >= later.dw.val(d) ? -7 : 0));
    }
  };
  later.dayOfYear = later.dy = {
    name: "day of year",
    range: 86400,
    val: function (d) {
      return d.dy || (d.dy = Math.ceil(1 + (later.D.start(d).getTime() - later.Y.start(d).getTime()) / later.DAY));
    },
    isValid: function (d, val) {
      return later.dy.val(d) === (val || later.dy.extent(d)[1]);
    },
    extent: function (d) {
      var year = later.Y.val(d);
      return d.dyExtent || (d.dyExtent = [1, year % 4 ? 365 : 366]);
    },
    start: function (d) {
      return later.D.start(d);
    },
    end: function (d) {
      return later.D.end(d);
    },
    next: function (d, val) {
      val = val > later.dy.extent(d)[1] ? 1 : val;
      var year = later.date.nextRollover(d, val, later.dy, later.Y),
        dyMax = later.dy.extent(year)[1];
      val = val > dyMax ? 1 : val || dyMax;
      return later.date.next(later.Y.val(year), later.M.val(year), val);
    },
    prev: function (d, val) {
      var year = later.date.prevRollover(d, val, later.dy, later.Y),
        dyMax = later.dy.extent(year)[1];
      val = val > dyMax ? dyMax : val || dyMax;
      return later.date.prev(later.Y.val(year), later.M.val(year), val);
    }
  };
  later.hour = later.h = {
    name: "hour",
    range: 3600,
    val: function (d) {
      return d.h || (d.h = later.date.getHour.call(d));
    },
    isValid: function (d, val) {
      return later.h.val(d) === val;
    },
    extent: function () {
      return [0, 23];
    },
    start: function (d) {
      return d.hStart || (d.hStart = later.date.next(later.Y.val(d), later.M.val(d), later.D.val(d), later.h.val(d)));
    },
    end: function (d) {
      return d.hEnd || (d.hEnd = later.date.prev(later.Y.val(d), later.M.val(d), later.D.val(d), later.h.val(d)));
    },
    next: function (d, val) {
      val = val > 23 ? 0 : val;
      var next = later.date.next(later.Y.val(d), later.M.val(d), later.D.val(d) + (val <= later.h.val(d) ? 1 : 0), val);
      if (!later.date.isUTC && next.getTime() <= d.getTime()) {
        next = later.date.next(later.Y.val(next), later.M.val(next), later.D.val(next), val + 1);
      }
      return next;
    },
    prev: function (d, val) {
      val = val > 23 ? 23 : val;
      return later.date.prev(later.Y.val(d), later.M.val(d), later.D.val(d) + (val >= later.h.val(d) ? -1 : 0), val);
    }
  };
  later.minute = later.m = {
    name: "minute",
    range: 60,
    val: function (d) {
      return d.m || (d.m = later.date.getMin.call(d));
    },
    isValid: function (d, val) {
      return later.m.val(d) === val;
    },
    extent: function (d) {
      return [0, 59];
    },
    start: function (d) {
      return d.mStart || (d.mStart = later.date.next(later.Y.val(d), later.M.val(d), later.D.val(d), later.h.val(d), later.m.val(d)));
    },
    end: function (d) {
      return d.mEnd || (d.mEnd = later.date.prev(later.Y.val(d), later.M.val(d), later.D.val(d), later.h.val(d), later.m.val(d)));
    },
    next: function (d, val) {
      var m = later.m.val(d),
        s = later.s.val(d),
        inc = val > 59 ? 60 - m : val <= m ? 60 - m + val : val - m,
        next = new Date(d.getTime() + inc * later.MIN - s * later.SEC);
      if (!later.date.isUTC && next.getTime() <= d.getTime()) {
        next = new Date(d.getTime() + (inc + 120) * later.MIN - s * later.SEC);
      }
      return next;
    },
    prev: function (d, val) {
      val = val > 59 ? 59 : val;
      return later.date.prev(later.Y.val(d), later.M.val(d), later.D.val(d), later.h.val(d) + (val >= later.m.val(d) ? -1 : 0), val);
    }
  };
  later.month = later.M = {
    name: "month",
    range: 2629740,
    val: function (d) {
      return d.M || (d.M = later.date.getMonth.call(d) + 1);
    },
    isValid: function (d, val) {
      return later.M.val(d) === (val || 12);
    },
    extent: function () {
      return [1, 12];
    },
    start: function (d) {
      return d.MStart || (d.MStart = later.date.next(later.Y.val(d), later.M.val(d)));
    },
    end: function (d) {
      return d.MEnd || (d.MEnd = later.date.prev(later.Y.val(d), later.M.val(d)));
    },
    next: function (d, val) {
      val = val > 12 ? 1 : val || 12;
      return later.date.next(later.Y.val(d) + (val > later.M.val(d) ? 0 : 1), val);
    },
    prev: function (d, val) {
      val = val > 12 ? 12 : val || 12;
      return later.date.prev(later.Y.val(d) - (val >= later.M.val(d) ? 1 : 0), val);
    }
  };
  later.second = later.s = {
    name: "second",
    range: 1,
    val: function (d) {
      return d.s || (d.s = later.date.getSec.call(d));
    },
    isValid: function (d, val) {
      return later.s.val(d) === val;
    },
    extent: function () {
      return [0, 59];
    },
    start: function (d) {
      return d;
    },
    end: function (d) {
      return d;
    },
    next: function (d, val) {
      var s = later.s.val(d),
        inc = val > 59 ? 60 - s : val <= s ? 60 - s + val : val - s,
        next = new Date(d.getTime() + inc * later.SEC);
      if (!later.date.isUTC && next.getTime() <= d.getTime()) {
        next = new Date(d.getTime() + (inc + 7200) * later.SEC);
      }
      return next;
    },
    prev: function (d, val, cache) {
      val = val > 59 ? 59 : val;
      return later.date.prev(later.Y.val(d), later.M.val(d), later.D.val(d), later.h.val(d), later.m.val(d) + (val >= later.s.val(d) ? -1 : 0), val);
    }
  };
  later.time = later.t = {
    name: "time",
    range: 1,
    val: function (d) {
      return d.t || (d.t = later.h.val(d) * 3600 + later.m.val(d) * 60 + later.s.val(d));
    },
    isValid: function (d, val) {
      return later.t.val(d) === val;
    },
    extent: function () {
      return [0, 86399];
    },
    start: function (d) {
      return d;
    },
    end: function (d) {
      return d;
    },
    next: function (d, val) {
      val = val > 86399 ? 0 : val;
      var next = later.date.next(later.Y.val(d), later.M.val(d), later.D.val(d) + (val <= later.t.val(d) ? 1 : 0), 0, 0, val);
      if (!later.date.isUTC && next.getTime() < d.getTime()) {
        next = later.date.next(later.Y.val(next), later.M.val(next), later.D.val(next), later.h.val(next), later.m.val(next), val + 7200);
      }
      return next;
    },
    prev: function (d, val) {
      val = val > 86399 ? 86399 : val;
      return later.date.next(later.Y.val(d), later.M.val(d), later.D.val(d) + (val >= later.t.val(d) ? -1 : 0), 0, 0, val);
    }
  };
  later.weekOfMonth = later.wm = {
    name: "week of month",
    range: 604800,
    val: function (d) {
      return d.wm || (d.wm = (later.D.val(d) + (later.dw.val(later.M.start(d)) - 1) + (7 - later.dw.val(d))) / 7);
    },
    isValid: function (d, val) {
      return later.wm.val(d) === (val || later.wm.extent(d)[1]);
    },
    extent: function (d) {
      return d.wmExtent || (d.wmExtent = [1, (later.D.extent(d)[1] + (later.dw.val(later.M.start(d)) - 1) + (7 - later.dw.val(later.M.end(d)))) / 7]);
    },
    start: function (d) {
      return d.wmStart || (d.wmStart = later.date.next(later.Y.val(d), later.M.val(d), Math.max(later.D.val(d) - later.dw.val(d) + 1, 1)));
    },
    end: function (d) {
      return d.wmEnd || (d.wmEnd = later.date.prev(later.Y.val(d), later.M.val(d), Math.min(later.D.val(d) + (7 - later.dw.val(d)), later.D.extent(d)[1])));
    },
    next: function (d, val) {
      val = val > later.wm.extent(d)[1] ? 1 : val;
      var month = later.date.nextRollover(d, val, later.wm, later.M),
        wmMax = later.wm.extent(month)[1];
      val = val > wmMax ? 1 : val || wmMax;
      return later.date.next(later.Y.val(month), later.M.val(month), Math.max(1, (val - 1) * 7 - (later.dw.val(month) - 2)));
    },
    prev: function (d, val) {
      var month = later.date.prevRollover(d, val, later.wm, later.M),
        wmMax = later.wm.extent(month)[1];
      val = val > wmMax ? wmMax : val || wmMax;
      return later.wm.end(later.date.next(later.Y.val(month), later.M.val(month), Math.max(1, (val - 1) * 7 - (later.dw.val(month) - 2))));
    }
  };
  later.weekOfYear = later.wy = {
    name: "week of year (ISO)",
    range: 604800,
    val: function (d) {
      if (d.wy) return d.wy;
      var wThur = later.dw.next(later.wy.start(d), 5),
        YThur = later.dw.next(later.Y.prev(wThur, later.Y.val(wThur) - 1), 5);
      return d.wy = 1 + Math.ceil((wThur.getTime() - YThur.getTime()) / later.WEEK);
    },
    isValid: function (d, val) {
      return later.wy.val(d) === (val || later.wy.extent(d)[1]);
    },
    extent: function (d) {
      if (d.wyExtent) return d.wyExtent;
      var year = later.dw.next(later.wy.start(d), 5),
        dwFirst = later.dw.val(later.Y.start(year)),
        dwLast = later.dw.val(later.Y.end(year));
      return d.wyExtent = [1, dwFirst === 5 || dwLast === 5 ? 53 : 52];
    },
    start: function (d) {
      return d.wyStart || (d.wyStart = later.date.next(later.Y.val(d), later.M.val(d), later.D.val(d) - (later.dw.val(d) > 1 ? later.dw.val(d) - 2 : 6)));
    },
    end: function (d) {
      return d.wyEnd || (d.wyEnd = later.date.prev(later.Y.val(d), later.M.val(d), later.D.val(d) + (later.dw.val(d) > 1 ? 8 - later.dw.val(d) : 0)));
    },
    next: function (d, val) {
      val = val > later.wy.extent(d)[1] ? 1 : val;
      var wyThur = later.dw.next(later.wy.start(d), 5),
        year = later.date.nextRollover(wyThur, val, later.wy, later.Y);
      if (later.wy.val(year) !== 1) {
        year = later.dw.next(year, 2);
      }
      var wyMax = later.wy.extent(year)[1],
        wyStart = later.wy.start(year);
      val = val > wyMax ? 1 : val || wyMax;
      return later.date.next(later.Y.val(wyStart), later.M.val(wyStart), later.D.val(wyStart) + 7 * (val - 1));
    },
    prev: function (d, val) {
      var wyThur = later.dw.next(later.wy.start(d), 5),
        year = later.date.prevRollover(wyThur, val, later.wy, later.Y);
      if (later.wy.val(year) !== 1) {
        year = later.dw.next(year, 2);
      }
      var wyMax = later.wy.extent(year)[1],
        wyEnd = later.wy.end(year);
      val = val > wyMax ? wyMax : val || wyMax;
      return later.wy.end(later.date.next(later.Y.val(wyEnd), later.M.val(wyEnd), later.D.val(wyEnd) + 7 * (val - 1)));
    }
  };
  later.year = later.Y = {
    name: "year",
    range: 31556900,
    val: function (d) {
      return d.Y || (d.Y = later.date.getYear.call(d));
    },
    isValid: function (d, val) {
      return later.Y.val(d) === val;
    },
    extent: function () {
      return [1970, 2099];
    },
    start: function (d) {
      return d.YStart || (d.YStart = later.date.next(later.Y.val(d)));
    },
    end: function (d) {
      return d.YEnd || (d.YEnd = later.date.prev(later.Y.val(d)));
    },
    next: function (d, val) {
      return val > later.Y.val(d) && val <= later.Y.extent()[1] ? later.date.next(val) : later.NEVER;
    },
    prev: function (d, val) {
      return val < later.Y.val(d) && val >= later.Y.extent()[0] ? later.date.prev(val) : later.NEVER;
    }
  };
  later.fullDate = later.fd = {
    name: "full date",
    range: 1,
    val: function (d) {
      return d.fd || (d.fd = d.getTime());
    },
    isValid: function (d, val) {
      return later.fd.val(d) === val;
    },
    extent: function () {
      return [0, 3250368e7];
    },
    start: function (d) {
      return d;
    },
    end: function (d) {
      return d;
    },
    next: function (d, val) {
      return later.fd.val(d) < val ? new Date(val) : later.NEVER;
    },
    prev: function (d, val) {
      return later.fd.val(d) > val ? new Date(val) : later.NEVER;
    }
  };
  later.modifier = {};
  later.modifier.after = later.modifier.a = function (constraint, values) {
    var value = values[0];
    return {
      name: "after " + constraint.name,
      range: (constraint.extent(new Date())[1] - value) * constraint.range,
      val: constraint.val,
      isValid: function (d, val) {
        return this.val(d) >= value;
      },
      extent: constraint.extent,
      start: constraint.start,
      end: constraint.end,
      next: function (startDate, val) {
        if (val != value) val = constraint.extent(startDate)[0];
        return constraint.next(startDate, val);
      },
      prev: function (startDate, val) {
        val = val === value ? constraint.extent(startDate)[1] : value - 1;
        return constraint.prev(startDate, val);
      }
    };
  };
  later.modifier.before = later.modifier.b = function (constraint, values) {
    var value = values[values.length - 1];
    return {
      name: "before " + constraint.name,
      range: constraint.range * (value - 1),
      val: constraint.val,
      isValid: function (d, val) {
        return this.val(d) < value;
      },
      extent: constraint.extent,
      start: constraint.start,
      end: constraint.end,
      next: function (startDate, val) {
        val = val === value ? constraint.extent(startDate)[0] : value;
        return constraint.next(startDate, val);
      },
      prev: function (startDate, val) {
        val = val === value ? value - 1 : constraint.extent(startDate)[1];
        return constraint.prev(startDate, val);
      }
    };
  };
  later.compile = function (schedDef) {
    var constraints = [],
      constraintsLen = 0,
      tickConstraint;
    for (var key in schedDef) {
      var nameParts = key.split("_"),
        name = nameParts[0],
        mod = nameParts[1],
        vals = schedDef[key],
        constraint = mod ? later.modifier[mod](later[name], vals) : later[name];
      constraints.push({
        constraint: constraint,
        vals: vals
      });
      constraintsLen++;
    }
    constraints.sort(function (a, b) {
      var ra = a.constraint.range,
        rb = b.constraint.range;
      return rb < ra ? -1 : rb > ra ? 1 : 0;
    });
    tickConstraint = constraints[constraintsLen - 1].constraint;
    function compareFn(dir) {
      return dir === "next" ? function (a, b) {
        return a.getTime() > b.getTime();
      } : function (a, b) {
        return b.getTime() > a.getTime();
      };
    }
    return {
      start: function (dir, startDate) {
        var next = startDate,
          nextVal = later.array[dir],
          maxAttempts = 1e3,
          done;
        while (maxAttempts-- && !done && next) {
          done = true;
          for (var i = 0; i < constraintsLen; i++) {
            var constraint = constraints[i].constraint,
              curVal = constraint.val(next),
              extent = constraint.extent(next),
              newVal = nextVal(curVal, constraints[i].vals, extent);
            if (!constraint.isValid(next, newVal)) {
              next = constraint[dir](next, newVal);
              done = false;
              break;
            }
          }
        }
        if (next !== later.NEVER) {
          next = dir === "next" ? tickConstraint.start(next) : tickConstraint.end(next);
        }
        return next;
      },
      end: function (dir, startDate) {
        var result,
          nextVal = later.array[dir + "Invalid"],
          compare = compareFn(dir);
        for (var i = constraintsLen - 1; i >= 0; i--) {
          var constraint = constraints[i].constraint,
            curVal = constraint.val(startDate),
            extent = constraint.extent(startDate),
            newVal = nextVal(curVal, constraints[i].vals, extent),
            next;
          if (newVal !== undefined) {
            next = constraint[dir](startDate, newVal);
            if (next && (!result || compare(result, next))) {
              result = next;
            }
          }
        }
        return result;
      },
      tick: function (dir, date) {
        return new Date(dir === "next" ? tickConstraint.end(date).getTime() + later.SEC : tickConstraint.start(date).getTime() - later.SEC);
      },
      // PATCH
      tickSafe: function (dir, date) {
        return dir === "next" ? diffSecond(tickConstraint.end(date), 1) : diffSecond(tickConstraint.start(date), -1);
      },
      // EOF PATCH
      tickStart: function (date) {
        return tickConstraint.start(date);
      }
    };
  };
  later.schedule = function (sched) {
    if (!sched) throw new Error("Missing schedule definition.");
    if (!sched.schedules) throw new Error("Definition must include at least one schedule.");
    var schedules = [],
      schedulesLen = sched.schedules.length,
      exceptions = [],
      exceptionsLen = sched.exceptions ? sched.exceptions.length : 0;
    for (var i = 0; i < schedulesLen; i++) {
      schedules.push(later.compile(sched.schedules[i]));
    }
    for (var j = 0; j < exceptionsLen; j++) {
      exceptions.push(later.compile(sched.exceptions[j]));
    }
    function getInstances(dir, count, startDate, endDate, isRange) {
      var compare = compareFn(dir),
        loopCount = count,
        maxAttempts = 1e6,
        schedStarts = [],
        exceptStarts = [],
        next,
        end,
        results = [],
        isForward = dir === "next",
        lastResult,
        rStart = isForward ? 0 : 1,
        rEnd = isForward ? 1 : 0;
      startDate = startDate ? new Date(startDate) : new Date();
      if (!startDate || !startDate.getTime()) throw new Error("Invalid start date.");
      setNextStarts(dir, schedules, schedStarts, startDate);
      setRangeStarts(dir, exceptions, exceptStarts, startDate);
      while (maxAttempts-- && loopCount && (next = findNext(schedStarts, compare))) {
        if (endDate && compare(next, endDate)) {
          break;
        }
        if (exceptionsLen) {
          updateRangeStarts(dir, exceptions, exceptStarts, next);
          if (end = calcRangeOverlap(dir, exceptStarts, next)) {
            updateNextStarts(dir, schedules, schedStarts, end);
            continue;
          }
        }
        if (isRange) {
          var maxEndDate = calcMaxEndDate(exceptStarts, compare);
          end = calcEnd(dir, schedules, schedStarts, next, maxEndDate);
          var r = isForward ? [new Date(Math.max(startDate, next)), end ? new Date(endDate ? Math.min(end, endDate) : end) : undefined] : [end ? new Date(endDate ? Math.max(endDate, end.getTime() + later.SEC) : end.getTime() + later.SEC) : undefined, new Date(Math.min(startDate, next.getTime() + later.SEC))];
          if (lastResult && r[rStart].getTime() === lastResult[rEnd].getTime()) {
            lastResult[rEnd] = r[rEnd];
            loopCount++;
          } else {
            lastResult = r;
            results.push(lastResult);
          }
          if (!end) break;
          updateNextStarts(dir, schedules, schedStarts, end);
        } else {
          results.push(isForward ? new Date(Math.max(startDate, next)) : getStart(schedules, schedStarts, next, endDate));
          tickStarts(dir, schedules, schedStarts, next);
        }
        loopCount--;
      }
      for (var i = 0, len = results.length; i < len; i++) {
        var result = results[i];
        results[i] = Object.prototype.toString.call(result) === "[object Array]" ? [cleanDate(result[0]), cleanDate(result[1])] : cleanDate(result);
      }
      return results.length === 0 ? later.NEVER : count === 1 ? results[0] : results;
    }
    function cleanDate(d) {
      if (d instanceof Date && !isNaN(d.valueOf())) {
        return new Date(d);
      }
      return undefined;
    }
    function setNextStarts(dir, schedArr, startsArr, startDate) {
      for (var i = 0, len = schedArr.length; i < len; i++) {
        startsArr[i] = schedArr[i].start(dir, startDate);
      }
    }
    function updateNextStarts(dir, schedArr, startsArr, startDate) {
      var compare = compareFn(dir);
      for (var i = 0, len = schedArr.length; i < len; i++) {
        if (startsArr[i] && !compare(startsArr[i], startDate)) {
          startsArr[i] = schedArr[i].start(dir, startDate);
        }
      }
    }
    function setRangeStarts(dir, schedArr, rangesArr, startDate) {
      for (var i = 0, len = schedArr.length; i < len; i++) {
        var nextStart = schedArr[i].start(dir, startDate);
        if (!nextStart) {
          rangesArr[i] = later.NEVER;
        } else {
          rangesArr[i] = [nextStart, schedArr[i].end(dir, nextStart)];
        }
      }
    }
    function updateRangeStarts(dir, schedArr, rangesArr, startDate) {
      var compare = compareFn(dir);
      for (var i = 0, len = schedArr.length; i < len; i++) {
        if (rangesArr[i] && !compare(rangesArr[i][0], startDate)) {
          var nextStart = schedArr[i].start(dir, startDate);
          if (!nextStart) {
            rangesArr[i] = later.NEVER;
          } else {
            rangesArr[i] = [nextStart, schedArr[i].end(dir, nextStart)];
          }
        }
      }
    }
    function tickStarts(dir, schedArr, startsArr, startDate) {
      for (var i = 0, len = schedArr.length; i < len; i++) {
        if (startsArr[i] && startsArr[i].getTime() === startDate.getTime()) {
          // PATCH
          const newStart = schedArr[i].start(dir, schedArr[i].tick(dir, startDate));
          // if the calculated `newStart` is the same as previous, this is probably
          // an endless loop caused by the DST shift (the browser won't halt because later.js
          // has a limit of 100000 iterations, but the calculated schedules will be all wrong)
          if (newStart !== later.NEVER && newStart.getTime() === startsArr[i].getTime()) {
            // in such case we use our own "safe" implementation of the "tick" method
            // which performs symbolic date manipulation
            // (instead of advancing on milliseconds since epoch timeaxis)
            // the former seems to be slower, that's why we only switch to it
            // in the case of infinite loop
            startsArr[i] = schedArr[i].start(dir, schedArr[i].tickSafe(dir, startDate));
          } else {
            startsArr[i] = newStart;
          }
          // EOF PATCH
        }
      }
    }

    function getStart(schedArr, startsArr, startDate, minEndDate) {
      var result;
      for (var i = 0, len = startsArr.length; i < len; i++) {
        if (startsArr[i] && startsArr[i].getTime() === startDate.getTime()) {
          var start = schedArr[i].tickStart(startDate);
          if (minEndDate && start < minEndDate) {
            return minEndDate;
          }
          if (!result || start > result) {
            result = start;
          }
        }
      }
      return result;
    }
    function calcRangeOverlap(dir, rangesArr, startDate) {
      var compare = compareFn(dir),
        result;
      for (var i = 0, len = rangesArr.length; i < len; i++) {
        var range = rangesArr[i];
        if (range && !compare(range[0], startDate) && (!range[1] || compare(range[1], startDate))) {
          if (!result || compare(range[1], result)) {
            result = range[1];
          }
        }
      }
      return result;
    }
    function calcMaxEndDate(exceptsArr, compare) {
      var result;
      for (var i = 0, len = exceptsArr.length; i < len; i++) {
        if (exceptsArr[i] && (!result || compare(result, exceptsArr[i][0]))) {
          result = exceptsArr[i][0];
        }
      }
      return result;
    }
    function calcEnd(dir, schedArr, startsArr, startDate, maxEndDate) {
      var compare = compareFn(dir),
        result;
      for (var i = 0, len = schedArr.length; i < len; i++) {
        var start = startsArr[i];
        if (start && start.getTime() === startDate.getTime()) {
          var end = schedArr[i].end(dir, start);
          if (maxEndDate && (!end || compare(end, maxEndDate))) {
            return maxEndDate;
          }
          if (!result || compare(end, result)) {
            result = end;
          }
        }
      }
      return result;
    }
    function compareFn(dir) {
      return dir === "next" ? function (a, b) {
        return !b || a.getTime() > b.getTime();
      } : function (a, b) {
        return !a || b.getTime() > a.getTime();
      };
    }
    function findNext(arr, compare) {
      var next = arr[0];
      for (var i = 1, len = arr.length; i < len; i++) {
        if (arr[i] && compare(next, arr[i])) {
          next = arr[i];
        }
      }
      return next;
    }
    return {
      isValid: function (d) {
        return getInstances("next", 1, d, d) !== later.NEVER;
      },
      next: function (count, startDate, endDate) {
        return getInstances("next", count || 1, startDate, endDate);
      },
      prev: function (count, startDate, endDate) {
        return getInstances("prev", count || 1, startDate, endDate);
      },
      nextRange: function (count, startDate, endDate) {
        return getInstances("next", count || 1, startDate, endDate, true);
      },
      prevRange: function (count, startDate, endDate) {
        return getInstances("prev", count || 1, startDate, endDate, true);
      }
    };
  };
  later.setTimeout = function (fn, sched) {
    var s = later.schedule(sched),
      t;
    if (fn) {
      scheduleTimeout();
    }
    function scheduleTimeout() {
      var now = Date.now(),
        next = s.next(2, now);
      if (!next[0]) {
        t = undefined;
        return;
      }
      var diff = next[0].getTime() - now;
      if (diff < 1e3) {
        diff = next[1] ? next[1].getTime() - now : 1e3;
      }
      if (diff < 2147483647) {
        t = setTimeout(fn, diff);
      } else {
        t = setTimeout(scheduleTimeout, 2147483647);
      }
    }
    return {
      isDone: function () {
        return !t;
      },
      clear: function () {
        clearTimeout(t);
      }
    };
  };
  later.setInterval = function (fn, sched) {
    if (!fn) {
      return;
    }
    var t = later.setTimeout(scheduleTimeout, sched),
      done = t.isDone();
    function scheduleTimeout() {
      if (!done) {
        fn();
        t = later.setTimeout(scheduleTimeout, sched);
      }
    }
    return {
      isDone: function () {
        return t.isDone();
      },
      clear: function () {
        done = true;
        t.clear();
      }
    };
  };
  later.date = {};
  later.date.timezone = function (useLocalTime) {
    later.date.build = useLocalTime ? function (Y, M, D, h, m, s) {
      return new Date(Y, M, D, h, m, s);
    } : function (Y, M, D, h, m, s) {
      return new Date(Date.UTC(Y, M, D, h, m, s));
    };
    var get = useLocalTime ? "get" : "getUTC",
      d = Date.prototype;
    later.date.getYear = d[get + "FullYear"];
    later.date.getMonth = d[get + "Month"];
    later.date.getDate = d[get + "Date"];
    later.date.getDay = d[get + "Day"];
    later.date.getHour = d[get + "Hours"];
    later.date.getMin = d[get + "Minutes"];
    later.date.getSec = d[get + "Seconds"];
    later.date.isUTC = !useLocalTime;
  };
  later.date.UTC = function () {
    later.date.timezone(false);
  };
  later.date.localTime = function () {
    later.date.timezone(true);
  };
  later.date.UTC();
  later.SEC = 1e3;
  later.MIN = later.SEC * 60;
  later.HOUR = later.MIN * 60;
  later.DAY = later.HOUR * 24;
  later.WEEK = later.DAY * 7;
  later.DAYS_IN_MONTH = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
  later.NEVER = 0;
  later.date.next = function (Y, M, D, h, m, s) {
    return later.date.build(Y, M !== undefined ? M - 1 : 0, D !== undefined ? D : 1, h || 0, m || 0, s || 0);
  };
  later.date.nextRollover = function (d, val, constraint, period) {
    var cur = constraint.val(d),
      max = constraint.extent(d)[1];
    return (val || max) <= cur || val > max ? new Date(period.end(d).getTime() + later.SEC) : period.start(d);
  };
  later.date.prev = function (Y, M, D, h, m, s) {
    var len = arguments.length;
    M = len < 2 ? 11 : M - 1;
    D = len < 3 ? later.D.extent(later.date.next(Y, M + 1))[1] : D;
    h = len < 4 ? 23 : h;
    m = len < 5 ? 59 : m;
    s = len < 6 ? 59 : s;
    return later.date.build(Y, M, D, h, m, s);
  };
  later.date.prevRollover = function (d, val, constraint, period) {
    var cur = constraint.val(d);
    return val >= cur || !val ? period.start(period.prev(d, period.val(d) - 1)) : period.start(d);
  };
  later.parse = {};
  later.parse.cron = function (expr, hasSeconds) {
    var NAMES = {
      JAN: 1,
      FEB: 2,
      MAR: 3,
      APR: 4,
      MAY: 5,
      JUN: 6,
      JUL: 7,
      AUG: 8,
      SEP: 9,
      OCT: 10,
      NOV: 11,
      DEC: 12,
      SUN: 1,
      MON: 2,
      TUE: 3,
      WED: 4,
      THU: 5,
      FRI: 6,
      SAT: 7
    };
    var REPLACEMENTS = {
      "* * * * * *": "0/1 * * * * *",
      "@YEARLY": "0 0 1 1 *",
      "@ANNUALLY": "0 0 1 1 *",
      "@MONTHLY": "0 0 1 * *",
      "@WEEKLY": "0 0 * * 0",
      "@DAILY": "0 0 * * *",
      "@HOURLY": "0 * * * *"
    };
    var FIELDS = {
      s: [0, 0, 59],
      m: [1, 0, 59],
      h: [2, 0, 23],
      D: [3, 1, 31],
      M: [4, 1, 12],
      Y: [6, 1970, 2099],
      d: [5, 1, 7, 1]
    };
    function getValue(value, offset, max) {
      return isNaN(value) ? NAMES[value] || null : Math.min(+value + (offset || 0), max || 9999);
    }
    function cloneSchedule(sched) {
      var clone = {},
        field;
      for (field in sched) {
        if (field !== "dc" && field !== "d") {
          clone[field] = sched[field].slice(0);
        }
      }
      return clone;
    }
    function add(sched, name, min, max, inc) {
      var i = min;
      if (!sched[name]) {
        sched[name] = [];
      }
      while (i <= max) {
        if (sched[name].indexOf(i) < 0) {
          sched[name].push(i);
        }
        i += inc || 1;
      }
      sched[name].sort(function (a, b) {
        return a - b;
      });
    }
    function addHash(schedules, curSched, value, hash) {
      if (curSched.d && !curSched.dc || curSched.dc && curSched.dc.indexOf(hash) < 0) {
        schedules.push(cloneSchedule(curSched));
        curSched = schedules[schedules.length - 1];
      }
      add(curSched, "d", value, value);
      add(curSched, "dc", hash, hash);
    }
    function addWeekday(s, curSched, value) {
      var except1 = {},
        except2 = {};
      if (value === 1) {
        add(curSched, "D", 1, 3);
        add(curSched, "d", NAMES.MON, NAMES.FRI);
        add(except1, "D", 2, 2);
        add(except1, "d", NAMES.TUE, NAMES.FRI);
        add(except2, "D", 3, 3);
        add(except2, "d", NAMES.TUE, NAMES.FRI);
      } else {
        add(curSched, "D", value - 1, value + 1);
        add(curSched, "d", NAMES.MON, NAMES.FRI);
        add(except1, "D", value - 1, value - 1);
        add(except1, "d", NAMES.MON, NAMES.THU);
        add(except2, "D", value + 1, value + 1);
        add(except2, "d", NAMES.TUE, NAMES.FRI);
      }
      s.exceptions.push(except1);
      s.exceptions.push(except2);
    }
    function addRange(item, curSched, name, min, max, offset) {
      var incSplit = item.split("/"),
        inc = +incSplit[1],
        range = incSplit[0];
      if (range !== "*" && range !== "0") {
        var rangeSplit = range.split("-");
        min = getValue(rangeSplit[0], offset, max);
        max = getValue(rangeSplit[1], offset, max) || max;
      }
      add(curSched, name, min, max, inc);
    }
    function parse(item, s, name, min, max, offset) {
      var value,
        split,
        schedules = s.schedules,
        curSched = schedules[schedules.length - 1];
      if (item === "L") {
        item = min - 1;
      }
      if ((value = getValue(item, offset, max)) !== null) {
        add(curSched, name, value, value);
      } else if ((value = getValue(item.replace("W", ""), offset, max)) !== null) {
        addWeekday(s, curSched, value);
      } else if ((value = getValue(item.replace("L", ""), offset, max)) !== null) {
        addHash(schedules, curSched, value, min - 1);
      } else if ((split = item.split("#")).length === 2) {
        value = getValue(split[0], offset, max);
        addHash(schedules, curSched, value, getValue(split[1]));
      } else {
        addRange(item, curSched, name, min, max, offset);
      }
    }
    function isHash(item) {
      return item.indexOf("#") > -1 || item.indexOf("L") > 0;
    }
    function itemSorter(a, b) {
      return isHash(a) && !isHash(b) ? 1 : a - b;
    }
    function parseExpr(expr) {
      var schedule = {
          schedules: [{}],
          exceptions: []
        },
        components = expr.replace(/(\s)+/g, " ").split(" "),
        field,
        f,
        component,
        items;
      for (field in FIELDS) {
        f = FIELDS[field];
        component = components[f[0]];
        if (component && component !== "*" && component !== "?") {
          items = component.split(",").sort(itemSorter);
          var i,
            length = items.length;
          for (i = 0; i < length; i++) {
            parse(items[i], schedule, field, f[1], f[2], f[3]);
          }
        }
      }
      return schedule;
    }
    function prepareExpr(expr) {
      var prepared = expr.toUpperCase();
      return REPLACEMENTS[prepared] || prepared;
    }
    var e = prepareExpr(expr);
    return parseExpr(hasSeconds ? e : "0 " + e);
  };
  later.parse.recur = function () {
    var schedules = [],
      exceptions = [],
      cur,
      curArr = schedules,
      curName,
      values,
      every,
      modifier,
      applyMin,
      applyMax,
      i,
      last;
    function add(name, min, max) {
      name = modifier ? name + "_" + modifier : name;
      if (!cur) {
        curArr.push({});
        cur = curArr[0];
      }
      if (!cur[name]) {
        cur[name] = [];
      }
      curName = cur[name];
      if (every) {
        values = [];
        for (i = min; i <= max; i += every) {
          values.push(i);
        }
        last = {
          n: name,
          x: every,
          c: curName.length,
          m: max
        };
      }
      values = applyMin ? [min] : applyMax ? [max] : values;
      var length = values.length;
      for (i = 0; i < length; i += 1) {
        var val = values[i];
        if (curName.indexOf(val) < 0) {
          curName.push(val);
        }
      }
      values = every = modifier = applyMin = applyMax = 0;
    }
    return {
      schedules: schedules,
      exceptions: exceptions,
      on: function () {
        values = arguments[0] instanceof Array ? arguments[0] : arguments;
        return this;
      },
      every: function (x) {
        every = x || 1;
        return this;
      },
      after: function (x) {
        modifier = "a";
        values = [x];
        return this;
      },
      before: function (x) {
        modifier = "b";
        values = [x];
        return this;
      },
      first: function () {
        applyMin = 1;
        return this;
      },
      last: function () {
        applyMax = 1;
        return this;
      },
      time: function () {
        for (var i = 0, len = values.length; i < len; i++) {
          var split = values[i].split(":");
          if (split.length < 3) split.push(0);
          values[i] = +split[0] * 3600 + +split[1] * 60 + +split[2];
        }
        add("t");
        return this;
      },
      second: function () {
        add("s", 0, 59);
        return this;
      },
      minute: function () {
        add("m", 0, 59);
        return this;
      },
      hour: function () {
        add("h", 0, 23);
        return this;
      },
      dayOfMonth: function () {
        add("D", 1, applyMax ? 0 : 31);
        return this;
      },
      dayOfWeek: function () {
        add("d", 1, 7);
        return this;
      },
      onWeekend: function () {
        values = [1, 7];
        return this.dayOfWeek();
      },
      onWeekday: function () {
        values = [2, 3, 4, 5, 6];
        return this.dayOfWeek();
      },
      dayOfWeekCount: function () {
        add("dc", 1, applyMax ? 0 : 5);
        return this;
      },
      dayOfYear: function () {
        add("dy", 1, applyMax ? 0 : 366);
        return this;
      },
      weekOfMonth: function () {
        add("wm", 1, applyMax ? 0 : 5);
        return this;
      },
      weekOfYear: function () {
        add("wy", 1, applyMax ? 0 : 53);
        return this;
      },
      month: function () {
        add("M", 1, 12);
        return this;
      },
      year: function () {
        add("Y", 1970, 2450);
        return this;
      },
      fullDate: function () {
        for (var i = 0, len = values.length; i < len; i++) {
          values[i] = values[i].getTime();
        }
        add("fd");
        return this;
      },
      customModifier: function (id, vals) {
        var custom = later.modifier[id];
        if (!custom) throw new Error("Custom modifier " + id + " not recognized!");
        modifier = id;
        values = arguments[1] instanceof Array ? arguments[1] : [arguments[1]];
        return this;
      },
      customPeriod: function (id) {
        var custom = later[id];
        if (!custom) throw new Error("Custom time period " + id + " not recognized!");
        add(id, custom.extent(new Date())[0], custom.extent(new Date())[1]);
        return this;
      },
      startingOn: function (start) {
        return this.between(start, last.m);
      },
      between: function (start, end) {
        cur[last.n] = cur[last.n].splice(0, last.c);
        every = last.x;
        add(last.n, start, end);
        return this;
      },
      and: function () {
        cur = curArr[curArr.push({}) - 1];
        return this;
      },
      except: function () {
        curArr = exceptions;
        cur = null;
        return this;
      }
    };
  };
  later.parse.text = function (str) {
    var recur = later.parse.recur,
      pos = 0,
      input = "",
      error;
    var TOKENTYPES = {
      eof: /^$/,
      fullDate: /^(\d\d\d\d-\d\d-\d\dt\d\d:\d\d:\d\d)\b/,
      rank: /^((\d\d\d\d)|([2-5]?1(st)?|[2-5]?2(nd)?|[2-5]?3(rd)?|(0|[1-5]?[4-9]|[1-5]0|1[1-3])(th)?))\b/,
      time: /^((([0]?[1-9]|1[0-2]):[0-5]\d(\s)?(am|pm))|(([0]?\d|1\d|2[0-3]):[0-5]\d))\b/,
      dayName: /^((sun|mon|tue(s)?|wed(nes)?|thu(r(s)?)?|fri|sat(ur)?)(day)?)\b/,
      monthName: /^(jan(uary)?|feb(ruary)?|ma((r(ch)?)?|y)|apr(il)?|ju(ly|ne)|aug(ust)?|oct(ober)?|(sept|nov|dec)(ember)?)\b/,
      yearIndex: /^(\d\d\d\d)\b/,
      every: /^every\b/,
      after: /^after\b/,
      before: /^before\b/,
      second: /^(s|sec(ond)?(s)?)\b/,
      minute: /^(m|min(ute)?(s)?)\b/,
      hour: /^(h|hour(s)?)\b/,
      day: /^(day(s)?( of the month)?)\b/,
      dayInstance: /^day instance\b/,
      dayOfWeek: /^day(s)? of the week\b/,
      dayOfYear: /^day(s)? of the year\b/,
      weekOfYear: /^week(s)?( of the year)?\b/,
      weekOfMonth: /^week(s)? of the month\b/,
      weekday: /^weekday\b/,
      weekend: /^weekend\b/,
      month: /^month(s)?\b/,
      year: /^year(s)?\b/,
      between: /^between (the)?\b/,
      start: /^(start(ing)? (at|on( the)?)?)\b/,
      at: /^(at|@)\b/,
      and: /^(,|and\b)/,
      except: /^(except\b)/,
      also: /(also)\b/,
      first: /^(first)\b/,
      last: /^last\b/,
      "in": /^in\b/,
      of: /^of\b/,
      onthe: /^on the\b/,
      on: /^on\b/,
      through: /(-|^(to|through)\b)/
    };
    var NAMES = {
      jan: 1,
      feb: 2,
      mar: 3,
      apr: 4,
      may: 5,
      jun: 6,
      jul: 7,
      aug: 8,
      sep: 9,
      oct: 10,
      nov: 11,
      dec: 12,
      sun: 1,
      mon: 2,
      tue: 3,
      wed: 4,
      thu: 5,
      fri: 6,
      sat: 7,
      "1st": 1,
      fir: 1,
      "2nd": 2,
      sec: 2,
      "3rd": 3,
      thi: 3,
      "4th": 4,
      "for": 4
    };
    function t(start, end, text, type) {
      return {
        startPos: start,
        endPos: end,
        text: text,
        type: type
      };
    }
    function peek(expected) {
      var scanTokens = expected instanceof Array ? expected : [expected],
        whiteSpace = /\s+/,
        token,
        curInput,
        m,
        scanToken,
        start,
        len;
      scanTokens.push(whiteSpace);
      start = pos;
      while (!token || token.type === whiteSpace) {
        len = -1;
        curInput = input.substring(start);
        token = t(start, start, input.split(whiteSpace)[0]);
        var i,
          length = scanTokens.length;
        for (i = 0; i < length; i++) {
          scanToken = scanTokens[i];
          m = scanToken.exec(curInput);
          if (m && m.index === 0 && m[0].length > len) {
            len = m[0].length;
            token = t(start, start + len, curInput.substring(0, len), scanToken);
          }
        }
        if (token.type === whiteSpace) {
          start = token.endPos;
        }
      }
      return token;
    }
    function scan(expectedToken) {
      var token = peek(expectedToken);
      pos = token.endPos;
      return token;
    }
    function parseThroughExpr(tokenType) {
      var start = +parseTokenValue(tokenType),
        end = checkAndParse(TOKENTYPES.through) ? +parseTokenValue(tokenType) : start,
        nums = [];
      for (var i = start; i <= end; i++) {
        nums.push(i);
      }
      return nums;
    }
    function parseRanges(tokenType) {
      var nums = parseThroughExpr(tokenType);
      while (checkAndParse(TOKENTYPES.and)) {
        nums = nums.concat(parseThroughExpr(tokenType));
      }
      return nums;
    }
    function parseEvery(r) {
      var num, period, start, end;
      if (checkAndParse(TOKENTYPES.weekend)) {
        r.on(NAMES.sun, NAMES.sat).dayOfWeek();
      } else if (checkAndParse(TOKENTYPES.weekday)) {
        r.on(NAMES.mon, NAMES.tue, NAMES.wed, NAMES.thu, NAMES.fri).dayOfWeek();
      } else {
        num = parseTokenValue(TOKENTYPES.rank);
        r.every(num);
        period = parseTimePeriod(r);
        if (checkAndParse(TOKENTYPES.start)) {
          num = parseTokenValue(TOKENTYPES.rank);
          r.startingOn(num);
          parseToken(period.type);
        } else if (checkAndParse(TOKENTYPES.between)) {
          start = parseTokenValue(TOKENTYPES.rank);
          if (checkAndParse(TOKENTYPES.and)) {
            end = parseTokenValue(TOKENTYPES.rank);
            r.between(start, end);
          }
        }
      }
    }
    function parseOnThe(r) {
      if (checkAndParse(TOKENTYPES.first)) {
        r.first();
      } else if (checkAndParse(TOKENTYPES.last)) {
        r.last();
      } else {
        r.on(parseRanges(TOKENTYPES.rank));
      }
      parseTimePeriod(r);
    }
    function parseScheduleExpr(str) {
      pos = 0;
      input = str;
      error = -1;
      var r = recur();
      while (pos < input.length && error < 0) {
        var token = parseToken([TOKENTYPES.every, TOKENTYPES.after, TOKENTYPES.before, TOKENTYPES.onthe, TOKENTYPES.on, TOKENTYPES.of, TOKENTYPES["in"], TOKENTYPES.at, TOKENTYPES.and, TOKENTYPES.except, TOKENTYPES.also]);
        switch (token.type) {
          case TOKENTYPES.every:
            parseEvery(r);
            break;
          case TOKENTYPES.after:
            if (peek(TOKENTYPES.time).type !== undefined) {
              r.after(parseTokenValue(TOKENTYPES.time));
              r.time();
            } else if (peek(TOKENTYPES.fullDate).type !== undefined) {
              r.after(parseTokenValue(TOKENTYPES.fullDate));
              r.fullDate();
            } else {
              r.after(parseTokenValue(TOKENTYPES.rank));
              parseTimePeriod(r);
            }
            break;
          case TOKENTYPES.before:
            if (peek(TOKENTYPES.time).type !== undefined) {
              r.before(parseTokenValue(TOKENTYPES.time));
              r.time();
            } else if (peek(TOKENTYPES.fullDate).type !== undefined) {
              r.before(parseTokenValue(TOKENTYPES.fullDate));
              r.fullDate();
            } else {
              r.before(parseTokenValue(TOKENTYPES.rank));
              parseTimePeriod(r);
            }
            break;
          case TOKENTYPES.onthe:
            parseOnThe(r);
            break;
          case TOKENTYPES.on:
            r.on(parseRanges(TOKENTYPES.dayName)).dayOfWeek();
            break;
          case TOKENTYPES.of:
            r.on(parseRanges(TOKENTYPES.monthName)).month();
            break;
          case TOKENTYPES["in"]:
            r.on(parseRanges(TOKENTYPES.yearIndex)).year();
            break;
          case TOKENTYPES.at:
            r.on(parseTokenValue(TOKENTYPES.time)).time();
            while (checkAndParse(TOKENTYPES.and)) {
              r.on(parseTokenValue(TOKENTYPES.time)).time();
            }
            break;
          case TOKENTYPES.and:
            break;
          case TOKENTYPES.also:
            r.and();
            break;
          case TOKENTYPES.except:
            r.except();
            break;
          default:
            error = pos;
        }
      }
      return {
        schedules: r.schedules,
        exceptions: r.exceptions,
        error: error
      };
    }
    function parseTimePeriod(r) {
      var timePeriod = parseToken([TOKENTYPES.second, TOKENTYPES.minute, TOKENTYPES.hour, TOKENTYPES.dayOfYear, TOKENTYPES.dayOfWeek, TOKENTYPES.dayInstance, TOKENTYPES.day, TOKENTYPES.month, TOKENTYPES.year, TOKENTYPES.weekOfMonth, TOKENTYPES.weekOfYear]);
      switch (timePeriod.type) {
        case TOKENTYPES.second:
          r.second();
          break;
        case TOKENTYPES.minute:
          r.minute();
          break;
        case TOKENTYPES.hour:
          r.hour();
          break;
        case TOKENTYPES.dayOfYear:
          r.dayOfYear();
          break;
        case TOKENTYPES.dayOfWeek:
          r.dayOfWeek();
          break;
        case TOKENTYPES.dayInstance:
          r.dayOfWeekCount();
          break;
        case TOKENTYPES.day:
          r.dayOfMonth();
          break;
        case TOKENTYPES.weekOfMonth:
          r.weekOfMonth();
          break;
        case TOKENTYPES.weekOfYear:
          r.weekOfYear();
          break;
        case TOKENTYPES.month:
          r.month();
          break;
        case TOKENTYPES.year:
          r.year();
          break;
        default:
          error = pos;
      }
      return timePeriod;
    }
    function checkAndParse(tokenType) {
      var found = peek(tokenType).type === tokenType;
      if (found) {
        scan(tokenType);
      }
      return found;
    }
    function parseToken(tokenType) {
      var t = scan(tokenType);
      if (t.type) {
        t.text = convertString(t.text, tokenType);
      } else {
        error = pos;
      }
      return t;
    }
    function parseTokenValue(tokenType) {
      return parseToken(tokenType).text;
    }
    function convertString(str, tokenType) {
      var output = str;
      switch (tokenType) {
        case TOKENTYPES.time:
          var parts = str.split(/(:|am|pm)/),
            hour = parts[3] === "pm" && parts[0] < 12 ? parseInt(parts[0], 10) + 12 : parts[0],
            min = parts[2].trim();
          output = (hour.length === 1 ? "0" : "") + hour + ":" + min;
          break;
        case TOKENTYPES.rank:
          output = parseInt(/^\d+/.exec(str)[0], 10);
          break;
        case TOKENTYPES.monthName:
        case TOKENTYPES.dayName:
          output = NAMES[str.substring(0, 3)];
          break;
        case TOKENTYPES.fullDate:
          output = new Date(str.toUpperCase());
          break;
      }
      return output;
    }
    return parseScheduleExpr(str.toLowerCase());
  };
  return later;
}();
// Set the local time mode for "later" library
later.date.localTime();

//---------------------------------------------------------------------------------------------------------------------
/**
 * Given a single `Iterable`, returns an array of 2 iterables, mirroring the original one (which should not be used anymore).
 *
 * For example:
 *
 *     const gen = function* () { yield 1; yield 2; yield 3 }
 *
 *     const [ iterable1, iterable2 ] = split(gen())
 *     const [ iter1, iter2 ] = [
 *         iterable1[ Symbol.iterator ](),
 *         iterable2[ Symbol.iterator ]()
 *     ]
 *
 *     iter1.next() // 1
 *     iter2.next() // 1
 *     iter2.next() // 2
 *     iter2.next() // 3
 *     iter1.next() // 2
 *     iter1.next() // 3
 *     iter1.next() // done
 *     iter2.next() // done
 *
 * @param iterable
 */
function split(iterable) {
  const gen1Pending = [];
  const gen2Pending = [];
  let iterator;
  const gen1 = function* () {
    if (!iterator) iterator = iterable[Symbol.iterator]();
    while (true) {
      if (gen1Pending.length) {
        yield* gen1Pending;
        gen1Pending.length = 0;
      }
      if (!iterator) break;
      const {
        value,
        done
      } = iterator.next();
      if (done) {
        iterator = null;
        iterable = null;
        break;
      }
      gen2Pending.push(value);
      yield value;
    }
  };
  const gen2 = function* () {
    if (!iterator) iterator = iterable[Symbol.iterator]();
    while (true) {
      if (gen2Pending.length) {
        yield* gen2Pending;
        gen2Pending.length = 0;
      }
      if (!iterator) break;
      const {
        value,
        done
      } = iterator.next();
      if (done) {
        iterator = null;
        iterable = null;
        break;
      }
      gen1Pending.push(value);
      yield value;
    }
  };
  return [gen1(), gen2()];
}
//---------------------------------------------------------------------------------------------------------------------
function* inBatchesBySize(iterator, batchSize) {
  if (batchSize < 0) throw new Error("Batch size needs to a natural number");
  batchSize = batchSize | 0;
  const runningBatch = [];
  for (const el of iterator) {
    if (runningBatch.length === batchSize) {
      yield runningBatch;
      runningBatch.length = 0;
    }
    runningBatch.push(el);
  }
  if (runningBatch.length > 0) yield runningBatch;
}
//---------------------------------------------------------------------------------------------------------------------
function* filter(iterator, func) {
  let i = 0;
  for (const el of iterator) {
    if (func(el, i++)) yield el;
  }
}
//---------------------------------------------------------------------------------------------------------------------
function* drop(iterator, howMany) {
  let i = 0;
  for (const el of iterator) {
    if (++i > howMany) yield el;
  }
}
//---------------------------------------------------------------------------------------------------------------------
function every(iterator, func) {
  let i = 0;
  for (const el of iterator) {
    if (!func(el, i++)) return false;
  }
  return true;
}
//---------------------------------------------------------------------------------------------------------------------
function some(iterator, func) {
  let i = 0;
  for (const el of iterator) {
    if (func(el, i++)) return true;
  }
  return false;
}
//---------------------------------------------------------------------------------------------------------------------
function* map(iterator, func) {
  let i = 0;
  for (const el of iterator) yield func(el, i++);
}
//---------------------------------------------------------------------------------------------------------------------
function reduce(iterator, func, initialAcc) {
  let i = 0;
  let acc = initialAcc;
  for (const el of iterator) {
    acc = func(acc, el, i++);
  }
  return acc;
}
//---------------------------------------------------------------------------------------------------------------------
function* uniqueOnly(iterator) {
  const seen = new Set();
  for (const el of iterator) {
    if (!seen.has(el)) {
      seen.add(el);
      yield el;
    }
  }
}
//---------------------------------------------------------------------------------------------------------------------
function* uniqueOnlyBy(iterator, func) {
  const seen = new Set();
  for (const el of iterator) {
    const uniqueBy = func(el);
    if (!seen.has(uniqueBy)) {
      seen.add(uniqueBy);
      yield el;
    }
  }
}
//---------------------------------------------------------------------------------------------------------------------
function* takeWhile(iterator, func) {
  let i = 0;
  for (const el of iterator) {
    if (func(el, i++)) yield el;else return;
  }
}
//---------------------------------------------------------------------------------------------------------------------
function* concat(...iterators) {
  for (let i = 0; i < iterators.length; i++) yield* iterators[i];
}
//---------------------------------------------------------------------------------------------------------------------
function* concatIterable(iteratorsProducer) {
  for (const iterator of iteratorsProducer) yield* iterator;
}
//---------------------------------------------------------------------------------------------------------------------
// just a chained syntax sugar class
// note, that we either use a combination of `this.derive()` + this.iterable (which will clear the `this.iterable`)
// or, use just `this` as iterable, which will also clear the iterator
//
class ChainedIteratorClass {
  constructor(iterable) {
    this.iterable = undefined;
    if (!iterable) throw new Error("Require an iterable instance for chaining");
    this.iterable = iterable;
  }
  derive(iterable) {
    this.iterable = undefined;
    return new ChainedIteratorClass(iterable);
  }
  copy() {
    const [iter1, iter2] = split(this.iterable);
    this.iterable = iter2;
    return new ChainedIteratorClass(iter1);
  }
  split() {
    const [iter1, iter2] = split(this.iterable);
    return [new ChainedIteratorClass(iter1), this.derive(iter2)];
  }
  inBatchesBySize(batchSize) {
    return this.derive(inBatchesBySize(this.iterable, batchSize));
  }
  filter(func) {
    return this.derive(filter(this.iterable, func));
  }
  drop(howMany) {
    return this.derive(drop(this.iterable, howMany));
  }
  map(func) {
    return this.derive(map(this.iterable, func));
  }
  reduce(func, initialAcc) {
    return reduce(this, func, initialAcc);
  }
  concat() {
    //@ts-ignore
    return this.derive(concatIterable(this.iterable));
  }
  uniqueOnly() {
    return this.derive(uniqueOnly(this.iterable));
  }
  uniqueOnlyBy(func) {
    return this.derive(uniqueOnlyBy(this.iterable, func));
  }
  every(func) {
    return every(this, func);
  }
  some(func) {
    return some(this, func);
  }
  takeWhile(func) {
    return this.derive(takeWhile(this.iterable, func));
  }
  *[Symbol.iterator]() {
    let iterable = this.iterable;
    if (!iterable) throw new Error("Chained iterator already exhausted or used to derive the new one");
    // practice shows, that cleaning up the iterable after yourself helps garbage collector a lot
    this.iterable = undefined;
    yield* iterable;
    // yes, we really want to avoid memory leaks
    iterable = undefined;
  }
  toArray() {
    return Array.from(this);
  }
  sort(order) {
    return Array.from(this).sort(order);
  }
  toSet() {
    return new Set(this);
  }
  toMap() {
    //@ts-ignore
    return new Map(this);
  }
  // toMap<K, V> () : T extends [ K, V ] ? Map<K, V> : never  {
  //     return new Map<K, V>(this.iterable as (T extends [ K, V ] ? Iterable<T> : never)) as (T extends [ K, V ] ? Map<K, V> : never)
  // }
  flush() {
    for (const element of this) {}
  }
  memoize() {
    return new MemoizedIteratorClass(this);
  }
}
const ChainedIterator = iterator => new ChainedIteratorClass(iterator);
const CI = ChainedIterator;
//---------------------------------------------------------------------------------------------------------------------
class MemoizedIteratorClass extends ChainedIteratorClass {
  constructor() {
    super(...arguments);
    this.elements = [];
    this.$iterator = undefined;
  }
  set iterable(iterable) {
    this.$iterable = iterable;
  }
  get iterable() {
    return this;
  }
  derive(iterable) {
    return new ChainedIteratorClass(iterable);
  }
  *[Symbol.iterator]() {
    const elements = this.elements;
    if (this.$iterable) {
      if (!this.$iterator) this.$iterator = this.$iterable[Symbol.iterator]();
      let iterator = this.$iterator;
      let alreadyConsumed = elements.length;
      // yield the 1st batch "efficiently"
      if (alreadyConsumed > 0) yield* elements;
      while (true) {
        if (elements.length > alreadyConsumed) {
          // wonder if `yield* elements.slice(alreadyConsumed)` is more performant or not
          for (let i = alreadyConsumed; i < elements.length; i++) yield elements[i];
          alreadyConsumed = elements.length;
        }
        if (!iterator) break;
        const {
          value,
          done
        } = iterator.next();
        if (done) {
          iterator = this.$iterator = null;
          this.$iterable = null;
        } else {
          elements.push(value);
          alreadyConsumed++;
          yield value;
        }
      }
    } else {
      yield* elements;
    }
  }
}
const MemoizedIterator = iterator => new MemoizedIteratorClass(iterator);
const MI = MemoizedIterator;

//---------------------------------------------------------------------------------------------------------------------
const MixinInstanceOfProperty = Symbol('MixinIdentity');
const MixinStateProperty = Symbol('MixinStateProperty');
//---------------------------------------------------------------------------------------------------------------------
class MixinWalkDepthState {
  constructor() {
    this.baseEl = undefined;
    this.sourceEl = undefined;
    this.$elementsByTopoLevel = undefined;
    this.$topoLevels = undefined;
    this.linearizedByTopoLevelsSource = MI(this.linearizedByTopoLevels());
  }
  static new(props) {
    const me = new this();
    props && Object.assign(me, props);
    return me;
  }
  get topoLevels() {
    if (this.$topoLevels !== undefined) return this.$topoLevels;
    return this.$topoLevels = this.buildTopoLevels();
  }
  buildTopoLevels() {
    return Array.from(this.elementsByTopoLevel.keys()).sort((level1, level2) => level1 - level2);
  }
  get elementsByTopoLevel() {
    if (this.$elementsByTopoLevel !== undefined) return this.$elementsByTopoLevel;
    return this.$elementsByTopoLevel = this.buildElementsByTopoLevel();
  }
  getOrCreateLevel(map, topoLevel) {
    let elementsAtLevel = map.get(topoLevel);
    if (!elementsAtLevel) {
      elementsAtLevel = [];
      map.set(topoLevel, elementsAtLevel);
    }
    return elementsAtLevel;
  }
  buildElementsByTopoLevel() {
    let maxTopoLevel = 0;
    const baseElements = this.baseEl ? CI(this.baseEl.walkDepthState.elementsByTopoLevel.values()).concat().toSet() : new Set();
    const map = CI(this.sourceEl.requirements).map(mixin => mixin.walkDepthState.elementsByTopoLevel).concat().reduce((elementsByTopoLevel, [topoLevel, mixins]) => {
      if (topoLevel > maxTopoLevel) maxTopoLevel = topoLevel;
      this.getOrCreateLevel(elementsByTopoLevel, topoLevel).push(mixins);
      return elementsByTopoLevel;
    }, new Map());
    this.getOrCreateLevel(map, maxTopoLevel + 1).push([this.sourceEl]);
    return CI(map).map(([level, elements]) => {
      return [level, CI(elements).concat().uniqueOnly().filter(mixin => !baseElements.has(mixin)).sort((mixin1, mixin2) => mixin1.id - mixin2.id)];
    }).toMap();
  }
  *linearizedByTopoLevels() {
    yield* CI(this.topoLevels).map(level => this.elementsByTopoLevel.get(level)).concat();
  }
}
// Note: 65535 mixins only, because of the hashing function implementation (String.fromCharCode)
let MIXIN_ID = 1;
//---------------------------------------------------------------------------------------------------------------------
/**
 * @hidden
 */
const identity = a => class extends a {};
// export type IdentityMixin<Base extends object>         = < T extends AnyConstructor<Base>>(base : T) => T
//
// export const IdentityMixin             = <Base extends object>() : IdentityMixin<Base> => identity
//---------------------------------------------------------------------------------------------------------------------
/**
 * @hidden
 */
class ZeroBaseClass {}
//---------------------------------------------------------------------------------------------------------------------
class MixinState {
  constructor() {
    this.id = MIXIN_ID++;
    this.requirements = [];
    this.baseClass = ZeroBaseClass;
    this.identitySymbol = undefined;
    this.mixinLambda = identity;
    this.walkDepthState = undefined;
    // private $hash               : MixinHash             = ''
    this.$minimalClass = undefined;
    this.name = '';
  }
  static new(props) {
    const me = new this();
    props && Object.assign(me, props);
    me.walkDepthState = MixinWalkDepthState.new({
      sourceEl: me,
      baseEl: getMixinState(me.baseClass)
    });
    //------------------
    const mixinLambda = me.mixinLambda;
    const symbol = me.identitySymbol = Symbol(mixinLambda.name);
    const mixinLambdaWrapper = Object.assign(function (base) {
      const extendedClass = mixinLambda(base);
      extendedClass.prototype[symbol] = true;
      return extendedClass;
    }, {
      [MixinInstanceOfProperty]: symbol,
      [MixinStateProperty]: me
    });
    Object.defineProperty(mixinLambdaWrapper, Symbol.hasInstance, {
      value: isInstanceOfStatic
    });
    me.mixinLambda = mixinLambdaWrapper;
    return me;
  }
  get minimalClass() {
    if (this.$minimalClass !== undefined) return this.$minimalClass;
    return this.$minimalClass = this.buildMinimalClass();
  }
  // get hash () : MixinHash {
  //     if (this.$hash !== '') return this.$hash
  //
  //     return this.$hash = this.buildHash()
  // }
  // buildHash () : MixinHash {
  //     return String.fromCharCode(...this.walkDepthState.linearizedByTopoLevelsSource.map(mixin => mixin.id))
  // }
  getBaseClassMixinId(baseClass) {
    const constructor = this.constructor;
    const mixinId = constructor.baseClassesIds.get(baseClass);
    if (mixinId !== undefined) return mixinId;
    const newId = MIXIN_ID++;
    constructor.baseClassesIds.set(baseClass, newId);
    return newId;
  }
  buildMinimalClass() {
    const self = this.constructor;
    let baseCls = this.baseClass;
    const minimalClassConstructor = this.walkDepthState.linearizedByTopoLevelsSource.reduce((acc, mixin) => {
      const {
        cls,
        hash
      } = acc;
      const nextHash = hash + String.fromCharCode(mixin.id);
      let wrapperCls = self.minimalClassesByLinearHash.get(nextHash);
      if (!wrapperCls) {
        wrapperCls = mixin.mixinLambda(cls);
        mixin.name = wrapperCls.name;
        self.minimalClassesByLinearHash.set(nextHash, wrapperCls);
      }
      acc.cls = wrapperCls;
      acc.hash = nextHash;
      return acc;
    }, {
      cls: baseCls,
      hash: String.fromCharCode(this.getBaseClassMixinId(baseCls))
    }).cls;
    const minimalClass = Object.assign(minimalClassConstructor, {
      [MixinInstanceOfProperty]: this.identitySymbol,
      [MixinStateProperty]: this,
      mix: this.mixinLambda,
      derive: base => Mixin([minimalClass, base], base => class extends base {}),
      $: this,
      toString: this.toString.bind(this)
    });
    Object.defineProperty(minimalClass, Symbol.hasInstance, {
      value: isInstanceOfStatic
    });
    return minimalClass;
  }
  toString() {
    return this.walkDepthState.linearizedByTopoLevelsSource.reduce((acc, mixin) => `${mixin.name}(${acc})`, this.baseClass.name);
  }
}
MixinState.minimalClassesByLinearHash = new Map();
MixinState.baseClassesIds = new Map();
//endregion type helpers
//---------------------------------------------------------------------------------------------------------------------
const isMixinClass = func => {
  return Object.getPrototypeOf(func.prototype).constructor.hasOwnProperty(MixinStateProperty);
};
const getMixinState = func => {
  return Object.getPrototypeOf(func.prototype).constructor[MixinStateProperty];
};
//---------------------------------------------------------------------------------------------------------------------
const mixin = (required, mixinLambda) => {
  let baseClass;
  if (required.length > 0) {
    const lastRequirement = required[required.length - 1];
    // absence of `[ MixinStateProperty ]` indicates its a regular class and not a mixin class
    // avoid assigning ZeroBaseClass - it will be applied as default at the end
    if (!isMixinClass(lastRequirement) && lastRequirement !== ZeroBaseClass) baseClass = lastRequirement;
  }
  const requirements = [];
  required.forEach((requirement, index) => {
    const mixinState = requirement[MixinStateProperty];
    if (mixinState !== undefined) {
      const currentBaseClass = mixinState.baseClass;
      // ignore ZeroBaseClass - since those are compatible with any other base class
      if (currentBaseClass !== ZeroBaseClass) {
        if (baseClass) {
          // already found a base class from requirements earlier
          if (baseClass !== currentBaseClass) {
            const currentIsSub = currentBaseClass.prototype.isPrototypeOf(baseClass.prototype);
            const currentIsSuper = baseClass.prototype.isPrototypeOf(currentBaseClass.prototype);
            if (!currentIsSub && !currentIsSuper) throw new Error("Base class mismatch");
            baseClass = currentIsSuper ? currentBaseClass : baseClass;
          }
        } else
          // first base class from requirements
          baseClass = currentBaseClass;
      }
      requirements.push(mixinState);
    } else {
      if (index !== required.length - 1) throw new Error("Base class should be provided as the last element of the requirements array");
    }
  });
  //------------------
  const mixinState = MixinState.new({
    requirements,
    mixinLambda: mixinLambda,
    baseClass: baseClass || ZeroBaseClass
  });
  return mixinState.minimalClass;
};
//---------------------------------------------------------------------------------------------------------------------
// this function works both with default mixin class and mixin application function
// it supplied internally as [Symbol.hasInstance] for the default mixin class and mixin application function
const isInstanceOfStatic = function (instance) {
  return Boolean(instance && instance[this[MixinInstanceOfProperty]]);
};
//---------------------------------------------------------------------------------------------------------------------
/**
 * This is the `instanceof` analog for the classes created with [[Mixin]] helper. It also provides [typeguard](https://www.typescriptlang.org/docs/handbook/advanced-types.html#user-defined-type-guards).
 *
 * There's no strict need to use it, as the native `instanceof` is also supported for the mixins created with the [[Mixin]] helper and also provides
 * typeguarding.
 *
 * @param instance Any value, normally an instance of the mixin class
 * @param func The constructor function of the class, created with [[Mixin]]
 */
const isInstanceOf = (instance, func) => {
  return Boolean(instance && instance[func[MixinInstanceOfProperty]]);
};
//---------------------------------------------------------------------------------------
/**
 * This function allows you to create mixin classes. Mixin classes solves the well-known problem with "classical" single-class inheritance,
 * in which class hierarchy must form a tree. When using mixins, class hierarchy becomes an arbitrary acyclic graph.
 *
 * Another view on mixins is that, if "classical" class is a point (a vertice of the graph), mixin class is an arrow between the points
 * (an edge in the graph, or rather, a description of the edge).
 *
 * Some background information about the mixin pattern can be found [here](https://mariusschulz.com/blog/typescript-2-2-mixin-classes)
 * and [here](https://www.bryntum.com/blog/the-mixin-pattern-in-typescript-all-you-need-to-know/).
 *
 * The pattern, being described here, is the evolution of the previous work, and main advantage is that it solves the compilation error
 * for circular references.
 *
 * Mixin definition. Requirements
 * ------------------------------
 *
 * The pattern looks like:
 *
 *     class Mixin1 extends Mixin(
 *         [],
 *         (base : AnyConstructor) =>
 *
 *         class Mixin1 extends base {
 *             prop1        : string
 *             method1 () : string {
 *                 return this.prop1
 *             }
 *             static static1 : number
 *         }
 *     ){}
 *
 * The core of the definition above is the mixin lambda - a function which receives a base class as its argument and returns a class,
 * extending the base class with additional properties.
 *
 * The example above creates a mixin `Mixin1` which has no requirements. Requirements are the other mixins,
 * which needs to be included in the base class of this mixin.
 *
 * There's also a special type of the requirement,
 * called "base class requirement". It is optional and can only appear as the last argument of the requirements
 * array. It does not have to be a mixin, created with the `Mixin` function, but can be any JS class. This requirement
 * specifies, that the base class of this mixin should be a subclass of the given class (or that class itself).
 *
 * The requirements of the mixin needs to be listed 3 times:
 * - as an array of constructor functions, in the 1st argument of the `Mixin` function
 * - as an instance type intersection, in the 1st type argument for the [[AnyConstructor]] type
 * - as an static type intersection, in the 2nd type argument for the [[AnyConstructor]] type
 *
 * For example, `Mixin2` requires `Mixin1`:
 *
 *     class Mixin2 extends Mixin(
 *         [ Mixin1 ],
 *         (base : AnyConstructor<Mixin1, typeof Mixin1>) =>
 *
 *         class Mixin2 extends base {
 *         }
 *     ){}
 *
 * And `Mixin3` requires both `Mixin1` and `Mixin2` (even that its redundant, since `Mixin2` already requires `Mixin1`,
 * but suppose we don't know the implementation details of the `Mixin2`):
 *
 *     class Mixin3 extends Mixin(
 *         [ Mixin1, Mixin2 ],
 *         (base : AnyConstructor<Mixin1 & Mixin2, typeof Mixin1 & typeof Mixin2>) =>
 *
 *         class Mixin3 extends base {
 *         }
 *     ){}
 *
 * Now, `Mixin4` requires `Mixin3`, plus, it requires the base class to be `SomeBaseClass`:
 *
 *     class SomeBaseClass {}
 *
 *     class Mixin4 extends Mixin(
 *         [ Mixin3, SomeBaseClass ],
 *         (base : AnyConstructor<
 *             Mixin3 & SomeBaseClass, typeof Mixin3 & typeof SomeBaseClass
 *         >) =>
 *
 *         class Mixin4 extends base {
 *         }
 *     ){}
 *
 * As already briefly mentioned, the requirements are "scanned" deep and included only once. Also all minimal classes are cached -
 * for example the creation of the Mixin3 will reuse the minimal class of the Mixin2 instead of creating a new intermediate class.
 * This means that all edges of the mixin dependencies graph are created only once (up to the base class).
 *
 * Requirements can not form cycles - that will generate both compilation error and run-time stack overflow.
 *
 * The typing for the `Mixin` function will provide a compilation error, if the requirements don't match, e.g. some requirement is
 * listed in the array, but missed in the types. This protects you from trivial mistakes. However, the typing is done up to 10 requirements only.
 * If you need more than 10 requirements for the mixin, use the [[MixinAny]] function, which is an exact analog of `Mixin`, but without
 * this type-level protection for requirements mismatch.
 *
 * It is possible to simplify the type of the `base` argument a bit, by using the [[ClassUnion]] helper. However, it seems in certain edge cases
 * it may lead to compilation errors. If your scenarios are not so complex you should give it a try. Using the [[ClassUnion]] helper, the
 * `Mixin3` can be defined as:
 *
 *     class Mixin3 extends Mixin(
 *         [ Mixin1, Mixin2 ],
 *         (base : ClassUnion<typeof Mixin1, typeof Mixin2>) =>
 *
 *         class Mixin3 extends base {
 *         }
 *     ){}
 *
 * Note, that due to this [issue](https://github.com/Microsoft/TypeScript/issues/7342), if you use decorators in your mixin class,
 * the declaration needs to be slightly more verbose (can not use compact notation for the arrow functions):
 *
 *     class Mixin2 extends Mixin(
 *         [ Mixin1 ],
 *         (base : AnyConstructor<Mixin1, typeof Mixin1>) => {
 *             class Mixin2 extends base {
 *                 @decorator
 *                 prop2 : string
 *             }
 *             return Mixin2
 *         }
 *     ){}
 *
 * As you noticed, the repeating listing of the requirements is somewhat verbose. Suggestions how the pattern can be improved
 * are [very welcomed](mailto:nickolay8@gmail.com).
 *
 * Mixin instantiation. Mixin constructor. `instanceof`
 * --------------------------------
 *
 * You can instantiate any mixin class just by using its constructor:
 *
 *     const instance1 = new Mixin1()
 *     const instance2 = new Mixin2()
 *
 * As explained in details [here](https://mariusschulz.com/blog/typescript-2-2-mixin-classes), mixin constructor should accept variable number of arguments
 * with the `any` type. This is simply because the mixin is supposed to be applicable to any other base class, which may have its own type
 * of the constructor arguments.
 *
 *     class Mixin2 extends Mixin(
 *         [ Mixin1 ],
 *         (base : AnyConstructor<Mixin1, typeof Mixin1>) => {
 *             class Mixin2 extends base {
 *                 prop2 : string
 *
 *                 constructor (...args: any[]) {
 *                     super(...args)
 *                     this.prop2 = ''
 *                 }
 *             }
 *             return Mixin2
 *         }
 *     ){}
 *
 * In other words, its not possible to provide any type-safety for mixin instantiation using regular class constructor.
 *
 * However, if we change the way we create class instances a little, we can get the type-safety back. For that,
 * we need to use a "uniform" class constructor - a constructor which has the same form for all classes. The [[Base]] class
 * provides such constructor as its static [[Base.new|new]] method. The usage of `Base` class is not required - you can use
 * any other base class.
 *
 * The `instanceof` operator works as expected for instances of the mixin classes. It also takes into account all the requirements.
 * For example:
 *
 *     const instance2 = new Mixin2()
 *
 *     const isMixin2 = instance2 instanceof Mixin2 // true
 *     const isMixin1 = instance2 instanceof Mixin1 // true, since Mixin2 requires Mixin1
 *
 * See also [[isInstanceOf]].
 *
 * "Manual" class derivation
 * --------------------------------
 *
 * You have defined a mixin using the `Mixin` function. Now you want to apply it to some base class to get the "specific" class to be able
 * to instantiate it. As described above - you don't have to, you can instantiate it directly.
 *
 * Sometimes however, you still want to derive the class "manually". For that, you can use static methods `mix` and `derive`, available
 * on all mixins.
 *
 * The `mix` method provides a direct access to the mixin lambda. It does not take requirements into account - that's the implementor's responsibility.
 * The `derive` method is something like "accumulated" mixin lambda - mixin lambda with all requirements.
 *
 * Both `mix` and `derive` provide the reasonably typed outcome.
 *
 *     class Mixin1 extends Mixin(
 *         [],
 *         (base : AnyConstructor) =>
 *
 *         class Mixin1 extends base {
 *             prop1        : string
 *         }
 *     ){}
 *
 *     class Mixin2 extends Mixin(
 *         [ Mixin1 ],
 *         (base : AnyConstructor<Mixin1, typeof Mixin1>) =>
 *
 *         class Mixin2 extends base {
 *             prop2        : string
 *         }
 *     ){}
 *
 *     const ManualMixin1 = Mixin1.mix(Object)
 *     const ManualMixin2 = Mixin2.mix(Mixin1.mix(Object))
 *
 *     const AnotherManualMixin1 = Mixin1.derive(Object)
 *     const AnotherManualMixin2 = Mixin2.derive(Object)
 *
 * Generics
 * --------
 *
 * Using generics with mixins is tricky because TypeScript does not have higher-kinded types and type inference for generics. Still some form
 * of generic arguments is possible, using the interface merging trick.
 *
 * Here's the pattern:
 *
 * ```ts
 * class Duplicator<Element> extends Mixin(
 *     [],
 *     (base : AnyConstructor) =>
 *
 *     class Duplicator extends base {
 *         Element                 : any
 *
 *         duplicate (value : this[ 'Element' ]) : this[ 'Element' ][] {
 *             return [ value, value ]
 *         }
 *     }
 * ){}
 *
 * interface Duplicator<Element> {
 *     Element : Element
 * }
 *
 * const dup = new Duplicator<boolean>()
 *
 * dup.duplicate('foo') // TS2345: Argument of type '"foo"' is not assignable to parameter of type 'boolean'.
 * ```
 *
 * In the example above, we've defined a generic argument `Element` for the outer mixin class, but in fact, that argument is not used anywhere in the
 * nested class definition in the mixin lambda. Instead, in the nested class, we define a property `Element`, which plays the role of the
 * generic argument.
 *
 * Mixin class methods then can refer to the generic type as `this[ 'Element' ]`.
 *
 * The generic arguments of the outer and nested classes are tied together in the additional interface declaration, which, by TypeScript rules
 * is merged together with the class definition. In this declaration, we specify that property `Element` has type of the `Element` generic argument.
 *
 * Limitations
 * ---------
 *
 * The most important limitation we found (which affect the old pattern as well) is the compilation error, which will be issued for
 * the private/protected methods, when compiling with declarations emitting (*.d.ts files generation).
 *
 * This is a [well-known problem](https://github.com/microsoft/TypeScript/issues/35822) in the TypeScript world – the *.d.ts files do not represent
 * the internal data structures of the TypeScript compiler well. Instead they use some simplified syntax, optimized for human editing.
 * This is why the compiler may generate false positives in the incremental compilation mode – it uses *.d.ts files internally.
 *
 * This can be a show-stopper for the people that use declaration files (usually for publishing). Keep in mind though, that you can always
 * publish actual TypeScript sources along with the generated JavaScript files, instead of publishing JavaScript + declarations files.
 *
 */
const Mixin = mixin;
/**
 * This is an exact analog of the [[Mixin]] function, but without type-level protection for requirements mismatch.
 * It supports unlimited number of requirements.
 */
const MixinAny = mixin;

/**
 * The enumeration for the time units
 */
var TimeUnit;
(function (TimeUnit) {
  TimeUnit["Millisecond"] = "millisecond";
  TimeUnit["Second"] = "second";
  TimeUnit["Minute"] = "minute";
  TimeUnit["Hour"] = "hour";
  TimeUnit["Day"] = "day";
  TimeUnit["Week"] = "week";
  TimeUnit["Month"] = "month";
  TimeUnit["Quarter"] = "quarter";
  TimeUnit["Year"] = "year";
})(TimeUnit || (TimeUnit = {}));
/**
 * The enumeration for the supported constraint types
 */
var ConstraintType;
(function (ConstraintType) {
  /**
   * "Must start on" constraint.
   * Restricts an event to start on a [[HasDateConstraintMixin.constraintDate|specified date]].
   * The constraint cannot be used for a summary event.
   */
  ConstraintType["MustStartOn"] = "muststarton";
  /**
   * "Must finish on" constraint.
   * Restricts an event to finish on a [[HasDateConstraintMixin.constraintDate|specified date]].
   * The constraint cannot be used for a summary event.
   */
  ConstraintType["MustFinishOn"] = "mustfinishon";
  /**
   * "Start no earlier than" constraint.
   * Restricting an event to start on or after a [[HasDateConstraintMixin.constraintDate|specified date]].
   */
  ConstraintType["StartNoEarlierThan"] = "startnoearlierthan";
  /**
   * "Start no later than" constraint.
   * Restricting an event to start on or before a [[HasDateConstraintMixin.constraintDate|specified date]].
   *
   * The constraint cannot be used for a summary task.
   */
  ConstraintType["StartNoLaterThan"] = "startnolaterthan";
  /**
   * "Finish no earlier than" constraint.
   * Restricting an event to finish on or after a [[HasDateConstraintMixin.constraintDate|specified date]].
   *
   * The constraint cannot be used for a summary task.
   */
  ConstraintType["FinishNoEarlierThan"] = "finishnoearlierthan";
  /**
   * "Finish no later than" constraint.
   * Restricting an event to finish on or before a [[HasDateConstraintMixin.constraintDate|specified date]].
   */
  ConstraintType["FinishNoLaterThan"] = "finishnolaterthan";
})(ConstraintType || (ConstraintType = {}));
/**
 * The enumeration for the supported scheduling modes
 */
var SchedulingMode;
(function (SchedulingMode) {
  SchedulingMode["Normal"] = "Normal";
  SchedulingMode["FixedDuration"] = "FixedDuration";
  SchedulingMode["FixedEffort"] = "FixedEffort";
  SchedulingMode["FixedUnits"] = "FixedUnits";
})(SchedulingMode || (SchedulingMode = {}));
/**
 * The enumeration for the dependency validation result
 */
var DependencyValidationResult;
(function (DependencyValidationResult) {
  /**
   * Dependency has no errors
   */
  DependencyValidationResult[DependencyValidationResult["NoError"] = 0] = "NoError";
  /**
   * Indicates that the validated dependency builds a cycle
   */
  DependencyValidationResult[DependencyValidationResult["CyclicDependency"] = 1] = "CyclicDependency";
  /**
   * Indicates that a dependency with the same predecessor and successor as validated one's already exists
   */
  DependencyValidationResult[DependencyValidationResult["DuplicatingDependency"] = 2] = "DuplicatingDependency";
})(DependencyValidationResult || (DependencyValidationResult = {}));
/**
 * The enumeration for the supported dependency types
 */
var DependencyType;
(function (DependencyType) {
  /**
   * Start-to-Start (_SS_)
   *
   * With this dependency type, the succeeding event is delayed to start not earlier than the preceding event starts.
   */
  DependencyType[DependencyType["StartToStart"] = 0] = "StartToStart";
  /**
   * Start-to-Finish (_SF_)
   *
   * The finish of the succeeding event is constrained by the start of the preceding event.
   * So the successor cannot finish before the predecessor starts.
   */
  DependencyType[DependencyType["StartToEnd"] = 1] = "StartToEnd";
  /**
   * Finish-to-Start (_FS_)
   *
   * This type of dependency, restricts the dependent event to not start earlier than the preceding event finishes.
   */
  DependencyType[DependencyType["EndToStart"] = 2] = "EndToStart";
  /**
   * Finish-to-Finish (_FF_)
   *
   * The succeeding event cannot finish before the completion of the preceding event.
   */
  DependencyType[DependencyType["EndToEnd"] = 3] = "EndToEnd";
})(DependencyType || (DependencyType = {}));
/**
 * The enumeration for the supported sources of the calendar for the dependency.
 */
var DependenciesCalendar;
(function (DependenciesCalendar) {
  DependenciesCalendar["Project"] = "Project";
  DependenciesCalendar["FromEvent"] = "FromEvent";
  DependenciesCalendar["ToEvent"] = "ToEvent";
})(DependenciesCalendar || (DependenciesCalendar = {}));
/**
 * Engine provides with different project types, the enumeration describes the types currently available
 */
var ProjectType;
(function (ProjectType) {
  ProjectType[ProjectType["SchedulerBasic"] = 1] = "SchedulerBasic";
  ProjectType[ProjectType["SchedulerPro"] = 2] = "SchedulerPro";
  ProjectType[ProjectType["Gantt"] = 3] = "Gantt";
})(ProjectType || (ProjectType = {}));
/**
 * The enumeration for the scheduling direction
 */
var Direction;
(function (Direction) {
  /**
   * Forward (or As Soon As Possible (ASAP)) scheduling.
   */
  Direction["Forward"] = "Forward";
  /**
   * Backward (or As Late As Possible (ALAP)) scheduling.
   */
  Direction["Backward"] = "Backward";
  Direction["None"] = "None";
})(Direction || (Direction = {}));
var ConstraintIntervalSide;
(function (ConstraintIntervalSide) {
  ConstraintIntervalSide["Start"] = "Start";
  ConstraintIntervalSide["End"] = "End";
})(ConstraintIntervalSide || (ConstraintIntervalSide = {}));

// http://ecma-international.org/ecma-262/5.1/#sec-15.9.1.1
/**
 * Minimal date representable with native Date class
 */
const MIN_DATE = new Date(-8640000000000000);
/**
 * Maximal date representable with native Date class
 */
const MAX_DATE = new Date(8640000000000000);
const isDateFinite = date => {
  if (!date) return false;
  const time = date.getTime();
  return time !== MIN_DATE.getTime() && time !== MAX_DATE.getTime();
};

/**
 * The date intervals in the scheduling engine are always inclusive on one end and opened on another.
 * The "opened" end is not considered to be a part of the interval.
 *
 * Depending from the scheduling direction (forward/backward) this property may need to be inverted.
 *
 * This enum specifies what edge of the interval is inclusive.
 */
var EdgeInclusion;
(function (EdgeInclusion) {
  EdgeInclusion[EdgeInclusion["Left"] = 0] = "Left";
  EdgeInclusion[EdgeInclusion["Right"] = 1] = "Right";
})(EdgeInclusion || (EdgeInclusion = {}));

/**
 * The enum type for result of [[forEachAvailabilityInterval]].
 */
var CalendarIteratorResult;
(function (CalendarIteratorResult) {
  /**
   * Indicates the iteration has completed by iterating the whole given timespan or has reached the MAX_DATE or MIN_DATE.
   */
  CalendarIteratorResult[CalendarIteratorResult["FullRangeIterated"] = 0] = "FullRangeIterated";
  /**
   * Indicates the iteration has been stopped by returning `false` from the iterator.
   */
  CalendarIteratorResult[CalendarIteratorResult["StoppedByIterator"] = 1] = "StoppedByIterator";
  /**
   * Indicates the iteration has exceeded the `maxRange` option
   */
  CalendarIteratorResult[CalendarIteratorResult["MaxCacheExtendCyclesReached"] = 2] = "MaxCacheExtendCyclesReached";
  /**
   * Indicates the iteration has exceeded the `maxRange` option
   */
  CalendarIteratorResult[CalendarIteratorResult["MaxRangeReached"] = 3] = "MaxRangeReached";
})(CalendarIteratorResult || (CalendarIteratorResult = {}));
/**
 * Calendar cache.
 */
class CalendarCache {
  constructor(config) {
    this.cacheFilledStartDate = MAX_DATE;
    this.cacheFilledEndDate = MIN_DATE;
    this.intervalsCachingChunkDuration = 30;
    this.intervalsCachingChunkUnit = TimeUnit.Day;
    this.maxCacheExtendCycles = 1000;
    // max range for the iteration - 5 years
    this.maxRange = 5 * 365 * 24 * 60 * 60 * 1000;
    config && Object.assign(this, config);
  }
  includeWrappingRangeFrom(cache, startDate, endDate) {
    cache.ensureCacheFilledForInterval(startDate, endDate);
    this.intervalCache.includeWrappingRange(cache.intervalCache, startDate, endDate);
  }
  // after this method, we guarantee, that for every point between `startDate` and `endDate` (_inclusive_)
  // we'll have a final representation of the cache, that is, we'll be able to get an interval to which this point belongs
  // _both_ for forward and backward directions
  ensureCacheFilledForInterval(startDate, endDate) {
    const cacheFilledStartDateN = this.cacheFilledStartDate.getTime();
    const cacheFilledEndDateN = this.cacheFilledEndDate.getTime();
    if (cacheFilledStartDateN !== MAX_DATE.getTime()) {
      const startDateN = startDate.getTime();
      const endDateN = endDate.getTime();
      if (cacheFilledStartDateN <= startDateN && endDateN <= cacheFilledEndDateN) return;
      // asked to cache an interval which is to the left from the cached area - extend to the right
      if (endDateN <= cacheFilledStartDateN) {
        endDate = new Date(cacheFilledStartDateN - 1);
      } else if (startDateN >= cacheFilledEndDateN) {
        startDate = new Date(cacheFilledEndDateN);
      } else if (cacheFilledStartDateN <= startDateN && startDateN <= cacheFilledEndDateN) {
        startDate = new Date(cacheFilledEndDateN + 1);
      } else if (cacheFilledStartDateN <= endDateN && endDateN <= cacheFilledEndDateN) {
        endDate = new Date(cacheFilledStartDateN - 1);
      } else {
        this.ensureCacheFilledForInterval(startDate, new Date(cacheFilledStartDateN - 1));
        this.ensureCacheFilledForInterval(new Date(cacheFilledEndDateN + 1), endDate);
        return;
      }
    }
    if (cacheFilledStartDateN === MAX_DATE.getTime() || startDate.getTime() < cacheFilledEndDateN) {
      this.cacheFilledStartDate = startDate;
    }
    if (cacheFilledEndDateN === MIN_DATE.getTime() || cacheFilledEndDateN < endDate.getTime()) {
      this.cacheFilledEndDate = endDate;
    }
    this.fillCache(startDate, endDate);
  }
  fillCache(_1 /* startDate */, _2 /* endDate */) {
    throw new Error("Abstract method");
  }
  clear() {
    this.cacheFilledStartDate = MAX_DATE;
    this.cacheFilledEndDate = MIN_DATE;
    this.intervalCache.clear();
  }
  /**
   * The core iterator method of the calendar cache.
   *
   * @param options The options for iterator. Should contain at least one of the `startDate`/`endDate` properties
   * which indicates what timespan to examine for availability intervals. If one of boundaries is not provided
   * iterator function should return `false` at some point, to avoid infinite loops.
   *
   * Another recognized option is `isForward`, which indicates the direction in which to iterate through the timespan.
   *
   * Another recognized option is `maxRange`, which indicates the maximum timespan for this iterator (in milliseconds). When iterator
   * exceeds this timespan, the iteration is stopped and [[CalendarIteratorResult.MaxRangeReached]] value is returned.
   * Default value is 5 years.
   *
   * @param func The iterator function to call. It will be called for every distinct set of availability intervals, found
   * in the given timespan. All the intervals, which are "active" for current interval are collected in the 3rd argument
   * for this function. If iterator returns `false` (checked with `===`) the iteration stops.
   *
   * @param scope The scope (`this` value) to execute the iterator in.
   */
  forEachAvailabilityInterval(options, func, scope) {
    scope = scope || this;
    const startDate = options.startDate;
    const endDate = options.endDate;
    const startDateN = startDate && startDate.getTime();
    const endDateN = endDate && endDate.getTime();
    const maxRange = options.maxRange ?? this.maxRange;
    // `isForward = true` by default
    const isForward = options.isForward !== false;
    if (isForward ? !startDate : !endDate) {
      throw new Error("At least `startDate` or `endDate` is required, depending from the `isForward` option");
    }
    const intervalCache = this.intervalCache;
    let cacheCursorDate = isForward ? startDate : endDate;
    let cursorDate = isForward ? startDate : endDate;
    const rangeStart = cursorDate.getTime();
    // this is generally an endless loop, but we artificially limit it to `maxCacheExtendCycles` iterations
    // to avoid freezing in unforeseen edge cases
    for (let cycle = 1; cycle < this.maxCacheExtendCycles; cycle++) {
      if (isForward) {
        this.ensureCacheFilledForInterval(cacheCursorDate, endDate || DateHelper.add(cacheCursorDate, this.intervalsCachingChunkDuration, this.intervalsCachingChunkUnit));
      } else {
        this.ensureCacheFilledForInterval(startDate || DateHelper.add(cacheCursorDate, -this.intervalsCachingChunkDuration, this.intervalsCachingChunkUnit), cacheCursorDate);
      }
      let interval = intervalCache.getIntervalOf(cursorDate, isForward ? EdgeInclusion.Left : EdgeInclusion.Right);
      while (interval) {
        const intervalStartDate = interval.startDate;
        const intervalEndDate = interval.endDate;
        // out of requested range - all done
        if (isForward && endDateN && intervalStartDate.getTime() >= endDateN || !isForward && startDateN && intervalEndDate.getTime() <= startDateN) {
          return CalendarIteratorResult.FullRangeIterated;
        }
        if (isForward && intervalStartDate.getTime() - rangeStart >= maxRange || !isForward && rangeStart - intervalEndDate.getTime() >= maxRange) {
          return CalendarIteratorResult.MaxRangeReached;
        }
        // we are out of cached area, need to extend the cache,
        // using `>=, <=` here, because, as an edge case the cache can be filled exactly up to that interval
        // and following interval will be infinity, which will trigger premature loop exit
        // caused by the https://github.com/bryntum/support/issues/6406
        if (isForward && intervalStartDate.getTime() >= this.cacheFilledEndDate.getTime() || !isForward && intervalEndDate.getTime() <= this.cacheFilledStartDate.getTime()) {
          break;
        }
        // save the last processed point, from which we should start after cache will be extended
        cursorDate = isForward ? intervalEndDate : intervalStartDate;
        // adjust to start / end date limits in iterator
        const countFrom = startDateN && intervalStartDate.getTime() < startDateN ? startDate : intervalStartDate;
        const countTill = endDateN && intervalEndDate.getTime() > endDateN ? endDate : intervalEndDate;
        if (func.call(scope, countFrom, countTill, interval.cacheInterval) === false) {
          // indicates premature exit if iterator returns `false`
          return CalendarIteratorResult.StoppedByIterator;
        }
        interval = isForward ? intervalCache.getNextInterval(interval) : intervalCache.getPrevInterval(interval);
      }
      if (isForward && cursorDate.getTime() === MAX_DATE.getTime() || !isForward && cursorDate.getTime() === MIN_DATE.getTime()) {
        return CalendarIteratorResult.FullRangeIterated;
      }
      cacheCursorDate = isForward ? this.cacheFilledEndDate : this.cacheFilledStartDate;
    }
    return CalendarIteratorResult.MaxCacheExtendCyclesReached;
  }
}

const stripDuplicates = array => Array.from(new Set(array));

/**
 * A class, that represent a cached set of availability intervals. One can use the [[getIsWorking]] method
 * to determine if this set intervals represents working time or non-working.
 */
class CalendarCacheInterval {
  constructor(config) {
    this.intervals = [];
    config && Object.assign(this, config);
    if (!this.calendar) throw new Error("Required attribute `calendar` is missing");
  }
  includeInterval(interval) {
    if (this.intervals.indexOf(interval) == -1) {
      const copy = this.intervals.slice();
      copy.push(interval);
      return new CalendarCacheInterval({
        intervals: copy,
        calendar: this.calendar
      });
    } else return this;
  }
  combineWith(interval) {
    return new CalendarCacheInterval({
      intervals: this.intervals.concat(interval.intervals),
      calendar: this.calendar
    });
  }
  /**
   * Returns the working status of this intervals set. It is determined as a working status
   * of the most prioritized interval (intervals are prioritized from child to parent)
   */
  getIsWorking() {
    if (this.isWorking != null) return this.isWorking;
    const intervals = this.intervals = this.normalizeIntervals(this.intervals);
    // return the value of the interval with the highest priority
    return this.isWorking = intervals[0].isWorking;
  }
  normalizeIntervals(intervals) {
    const filtered = stripDuplicates(intervals);
    // sort in decreasing order
    filtered.sort((interval1, interval2) => interval2.getPriorityField() - interval1.getPriorityField());
    return filtered;
  }
}

// Generic binary search
const binarySearch = (value, array, comparator = (a, b) => a - b) => {
  let left = 0;
  let right = array.length;
  while (left < right) {
    // | 0 to make it integer, faster according to: https://jsperf.com/or-vs-floor/2
    const mid = (left + right) / 2 | 0;
    const compare = comparator(value, array[mid]);
    if (compare === 0) return {
      found: true,
      index: mid
    };else if (compare < 0) right = mid;else left = mid + 1;
  }
  return {
    found: false,
    index: right
  };
};

var IndexPosition;
(function (IndexPosition) {
  IndexPosition[IndexPosition["Exact"] = 0] = "Exact";
  IndexPosition[IndexPosition["Next"] = 1] = "Next";
})(IndexPosition || (IndexPosition = {}));
class SortedMap {
  constructor(comparator) {
    this.keys = [];
    this.values = [];
    this.comparator = comparator || ((a, b) => a - b);
  }
  set(key, value) {
    const search = binarySearch(key, this.keys, this.comparator);
    if (search.found) {
      this.values[search.index] = value;
    } else {
      this.keys.splice(search.index, 0, key);
      this.values.splice(search.index, 0, value);
    }
    return search.index;
  }
  // you need to know what you are doing when using this method
  insertAt(index, key, value) {
    this.keys.splice(index, 0, key);
    this.values.splice(index, 0, value);
  }
  setValueAt(index, value) {
    this.values[index] = value;
  }
  get(key) {
    const search = binarySearch(key, this.keys, this.comparator);
    return search.found ? this.values[search.index] : undefined;
  }
  getEntryAt(index) {
    return index < this.keys.length ? {
      key: this.keys[index],
      value: this.values[index]
    } : undefined;
  }
  getKeyAt(index) {
    return this.keys[index];
  }
  getValueAt(index) {
    return this.values[index];
  }
  delete(key) {
    const search = binarySearch(key, this.keys, this.comparator);
    if (search.found) this.deleteAt(search.index);
  }
  size() {
    return this.keys.length;
  }
  deleteAt(index) {
    this.keys.splice(index, 1);
    this.values.splice(index, 1);
  }
  indexOfKey(key) {
    const search = binarySearch(key, this.keys, this.comparator);
    return {
      found: search.found ? IndexPosition.Exact : IndexPosition.Next,
      index: search.index
    };
  }
  map(func) {
    const keys = this.keys;
    const values = this.values;
    const result = [];
    for (let i = 0; i < keys.length; i++) result.push(func(values[i], keys[i], i));
    return result;
  }
  getAllEntries() {
    return this.map((value, key) => {
      return {
        value,
        key
      };
    });
  }
  clear() {
    this.keys.length = 0;
    this.values.length = 0;
  }
}

class IntervalCache {
  constructor(config) {
    this.points = new SortedMap((a, b) => a.getTime() - b.getTime());
    this.leftInfinityKey = MIN_DATE;
    this.rightInfinityKey = MAX_DATE;
    Object.assign(this, config);
    if (this.emptyInterval === undefined || !this.combineIntervalsFn) throw new Error("All of `emptyPoint`, `combineIntervalsFn` are required");
    this.points.set(this.leftInfinityKey, this.emptyInterval);
  }
  size() {
    return this.points.size();
  }
  indexOf(date) {
    return this.points.indexOfKey(date);
  }
  getDateAt(index) {
    return this.points.getKeyAt(index);
  }
  getPointAt(index) {
    return this.points.getValueAt(index);
  }
  getIntervalOf(date, edgeInclusion = EdgeInclusion.Left) {
    // the `index` here is guaranteed to be > 0, because at index 0 there's a `emptyPoint`
    let {
      found,
      index
    } = this.indexOf(date);
    let startDateIndex;
    if (edgeInclusion === EdgeInclusion.Left) {
      startDateIndex = found === IndexPosition.Exact ? index : index - 1;
    } else {
      startDateIndex = index - 1;
    }
    return this.getIntervalWithStartDateIndex(startDateIndex);
  }
  getPrevInterval(interval) {
    if (interval.startDateIndex === 0) return null;
    return this.getIntervalWithStartDateIndex(interval.startDateIndex - 1);
  }
  getNextInterval(interval) {
    if (interval.startDateIndex >= this.size() - 1) return null;
    return this.getIntervalWithStartDateIndex(interval.startDateIndex + 1);
  }
  getIntervalWithStartDateIndex(startDateIndex) {
    return {
      startDateIndex: startDateIndex,
      startDate: this.getDateAt(startDateIndex),
      endDate: startDateIndex + 1 < this.size() ? this.getDateAt(startDateIndex + 1) : this.rightInfinityKey,
      cacheInterval: this.getPointAt(startDateIndex)
    };
  }
  addInterval(startDate, endDate, extendInterval) {
    const points = this.points;
    // there is always "leftInfinityKey" empty point, so `index >= 0`
    const {
      found,
      index
    } = points.indexOfKey(startDate);
    let curIndex;
    let lastUpdatedPoint;
    if (found == IndexPosition.Exact) {
      const inclusion = extendInterval(lastUpdatedPoint = points.getValueAt(index));
      points.setValueAt(index, inclusion);
      curIndex = index + 1;
    } else {
      const inclusion = extendInterval(lastUpdatedPoint = points.getValueAt(index - 1));
      points.insertAt(index, startDate, inclusion);
      curIndex = index + 1;
    }
    while (curIndex < points.size()) {
      const curDate = points.getKeyAt(curIndex);
      if (curDate.getTime() >= endDate.getTime()) break;
      const inclusion = extendInterval(lastUpdatedPoint = points.getValueAt(curIndex));
      points.setValueAt(curIndex, inclusion);
      curIndex++;
    }
    if (curIndex === points.size()) {
      points.insertAt(points.size(), endDate, this.emptyInterval);
    } else {
      const curDate = points.getKeyAt(curIndex);
      if (curDate.getTime() === endDate.getTime()) ; else {
        points.insertAt(curIndex, endDate, lastUpdatedPoint);
      }
    }
  }
  includeWrappingRange(intervalCache, startDate, endDate) {
    let interval = intervalCache.getIntervalOf(startDate);
    while (interval) {
      this.addInterval(interval.startDate, interval.endDate, existingInterval => this.combineIntervalsFn(existingInterval, interval.cacheInterval));
      if (interval.endDate.getTime() > endDate.getTime()) break;
      interval = intervalCache.getNextInterval(interval);
    }
  }
  getSummary() {
    return this.points.map((label, date) => {
      return {
        label,
        date
      };
    });
  }
  clear() {
    this.points.clear();
    this.points.set(this.leftInfinityKey, this.emptyInterval);
  }
}

class CalendarCacheSingle extends CalendarCache {
  constructor(config) {
    super(config);
    this.staticIntervalsCached = false;
    if (!this.unspecifiedTimeInterval) throw new Error("Required attribute `unspecifiedTimeInterval` is missing");
    this.intervalCache = new IntervalCache({
      emptyInterval: new CalendarCacheInterval({
        intervals: [this.unspecifiedTimeInterval],
        calendar: this.calendar
      }),
      combineIntervalsFn: (interval1, interval2) => {
        return interval1.combineWith(interval2);
      }
    });
  }
  fillCache(startDate, endDate) {
    var _this$calendar$projec;
    if (!this.staticIntervalsCached) {
      this.cacheStaticIntervals();
      this.staticIntervalsCached = true;
    }
    if (this.parentCache) this.includeWrappingRangeFrom(this.parentCache, startDate, endDate);
    const startDateN = startDate.getTime();
    const endDateN = endDate.getTime();
    const timeZone = (_this$calendar$projec = this.calendar.project) === null || _this$calendar$projec === void 0 ? void 0 : _this$calendar$projec.timeZone;
    if (startDateN > endDateN) throw new Error("Invalid cache fill interval");
    this.forEachRecurrentInterval(interval => {
      const startSchedule = interval.getStartDateSchedule();
      const endSchedule = interval.getEndDateSchedule();
      let wrappingStartDate = startSchedule.prev(1, startDate);
      let wrappingEndDate;
      if (endSchedule === 'EOD') {
        const nextEndDate = startSchedule.next(1, endDate);
        if (nextEndDate !== later.NEVER) {
          wrappingEndDate = DateHelper.getStartOfNextDay(nextEndDate, true);
        } else {
          wrappingEndDate = later.NEVER;
        }
      } else {
        wrappingEndDate = endSchedule.next(1, endDate);
      }
      // if the `startDate` is an occurrence in the interval's schedule, we need to advance one point prior
      // this is to provide the backward-scheduling information for the `startDate` point
      if (wrappingStartDate !== later.NEVER && wrappingStartDate.getTime() === startDateN) {
        const wrappingStartDates = startSchedule.prev(2, startDate);
        if (wrappingStartDates !== later.NEVER && wrappingStartDates.length === 2) wrappingStartDate = wrappingStartDates[1];
      }
      if (wrappingEndDate !== later.NEVER && wrappingEndDate.getTime() === endDateN) {
        const wrappingEndDates = endSchedule.next(2, endDate);
        if (wrappingEndDates !== later.NEVER && wrappingEndDates.length === 2) wrappingEndDate = wrappingEndDates[1];
      }
      const startDates = startSchedule.next(Infinity, wrappingStartDate !== later.NEVER ? wrappingStartDate : startDate, wrappingEndDate !== later.NEVER ? new Date(wrappingEndDate.getTime() - 1) : endDate);
      // schedule is empty for the interval of interest, do nothing
      if (startDates === later.NEVER) return;
      // at this point `startDates` is a non-empty array
      const endDates = endSchedule === 'EOD' ? startDates.map(date => DateHelper.getStartOfNextDay(date, true)) : endSchedule.next(Infinity, new Date(startDates[0].getTime() + 1), wrappingEndDate !== later.NEVER ? wrappingEndDate : endDate);
      if (endDates === later.NEVER) return;
      if (endDates.length > startDates.length) {
        // safe to ignore "extra" end dates
        endDates.length = startDates.length;
      } else if (endDates.length < startDates.length) {
        // monkey patch
        startDates.length = endDates.length;
        // throw new Error("Recurrent interval inconsistency: " + interval + ", caching startDate: " + startDate + ", caching endDate: " + endDate)
      }

      startDates.forEach((startDate, index) => {
        let recStartDate = startDate;
        let recEndDate = endDates[index];
        // Adjust calendar intervals when changing time zone
        if (timeZone != null) {
          recStartDate = TimeZoneHelper.toTimeZone(recStartDate, timeZone);
          recEndDate = TimeZoneHelper.toTimeZone(recEndDate, timeZone);
        }
        // if (recStartDate.getTime() > recEndDate.getTime())
        //     throw new Error("Recurrent interval inconsistency: " + interval + ", startDate: " + startDate + ", endDate: " + endDates[ index ])
        this.intervalCache.addInterval(recStartDate, recEndDate, existingCacheInterval => existingCacheInterval.includeInterval(interval));
      });
    });
  }
  clear() {
    this.staticIntervalsCached = false;
    super.clear();
  }
  cacheStaticIntervals() {
    this.forEachStaticInterval(interval => {
      var _this$calendar$projec2;
      const timeZone = (_this$calendar$projec2 = this.calendar.project) === null || _this$calendar$projec2 === void 0 ? void 0 : _this$calendar$projec2.timeZone;
      let {
        startDate,
        endDate
      } = interval;
      // Adjust calendar intervals when changing time zone
      if (timeZone != null) {
        startDate = TimeZoneHelper.toTimeZone(startDate, timeZone);
        endDate = TimeZoneHelper.toTimeZone(endDate, timeZone);
      }
      this.intervalCache.addInterval(startDate, endDate, existingCacheInterval => existingCacheInterval.includeInterval(interval));
    });
  }
  forEachStaticInterval(func) {
    this.intervalStore.forEach(interval => {
      if (interval.isStatic()) func(interval);
    });
  }
  forEachRecurrentInterval(func) {
    this.intervalStore.forEach(interval => {
      if (interval.isRecurrent()) func(interval);
    });
  }
}

/**
 * This a base generic mixin for every class, that belongs to a project.
 *
 * It just provides getter/setter for the `project` property, along with some convenience methods
 * to access the project's stores.
 */
class AbstractPartOfProjectGenericMixin extends Mixin([], base => {
  base.prototype;
  class AbstractPartOfProjectGenericMixin extends base {
    async commitAsync() {
      return this.project.commitAsync();
    }
    set project(project) {
      this.$project = project;
    }
    get project() {
      return this.$project;
    }
    calculateProject() {
      throw new Error("Implement me");
    }
    /**
     * The method to set the [[AbstractProjectMixin|project]] instance, this entity belongs to.
     */
    setProject(project) {
      return this.project = project;
    }
    /**
     * The method to get the [[AbstractProjectMixin|project]] instance, this entity belongs to.
     */
    getProject() {
      if (this.project) return this.project;
      return this.setProject(this.calculateProject());
    }
    /**
     * Convenience method to get the instance of the assignment store in the [[AbstractProjectMixin|project]] instance, this entity belongs to.
     */
    getAssignmentStore() {
      const project = this.getProject();
      return project === null || project === void 0 ? void 0 : project.assignmentStore;
    }
    /**
     * Convenience method to get the instance of the dependency store in the [[AbstractProjectMixin|project]] instance, this entity belongs to.
     */
    getDependencyStore() {
      const project = this.getProject();
      return project === null || project === void 0 ? void 0 : project.dependencyStore;
    }
    /**
     * Convenience method to get the instance of the event store in the [[AbstractProjectMixin|project]] instance, this entity belongs to.
     */
    getEventStore() {
      const project = this.getProject();
      return project === null || project === void 0 ? void 0 : project.eventStore;
    }
    /**
     * Convenience method to get the instance of the resource store in the [[AbstractProjectMixin|project]] instance, this entity belongs to.
     */
    getResourceStore() {
      const project = this.getProject();
      return project === null || project === void 0 ? void 0 : project.resourceStore;
    }
    /**
     * Convenience method to get the instance of the calendar manager store in the [[AbstractProjectMixin|project]] instance, this entity belongs to.
     */
    getCalendarManagerStore() {
      const project = this.getProject();
      return project === null || project === void 0 ? void 0 : project.calendarManagerStore;
    }
  }
  return AbstractPartOfProjectGenericMixin;
}) {}

/**
 * This an abstract mixin for every Store, that belongs to a project.
 *
 * The store with this mixin, supposes, that it will be "joining" the project, a reference to which is saved
 * and made available for all models.
 */
class AbstractPartOfProjectStoreMixin extends Mixin([AbstractPartOfProjectGenericMixin, Store], base => {
  const superProto = base.prototype;
  class AbstractPartOfProjectStoreMixin extends base {
    constructor() {
      super(...arguments);
      this.isLoadingData = false;
      this.disableHasLoadedDataToCommitFlag = false;
    }
    //region Async event triggering
    // NOTE: Tested in Scheduler (EventStore.t.js)
    construct(config = {}) {
      config.asyncEvents = {
        add: true,
        remove: true,
        removeAll: true,
        change: true,
        refresh: true,
        replace: true,
        move: true,
        update: true
      };
      return superProto.construct.call(this, config);
    }
    // Override for event triggering, to allow triggering events before and after some async operation.
    // The "before" events are prefix, the "after" are not.
    trigger(eventName, param) {
      const me = this,
        {
          asyncEvents,
          project
        } = me,
        asyncEvent = asyncEvents === null || asyncEvents === void 0 ? void 0 : asyncEvents[eventName],
        asyncAction = asyncEvent && (asyncEvent === true || asyncEvent[param.action]);
      if (!asyncAction) {
        // Trigger as usual
        return superProto.trigger.call(me, eventName, param);
      }
      // Trigger prefixed before event
      superProto.trigger.call(me, `${eventName}PreCommit`, {
        ...param
      });
      // Event that did not invalidate engine, for example "update"
      if (!project || project.isEngineReady() && !project.isWritingData) {
        // Trigger "original" event
        superProto.trigger.call(me, eventName, param);
      } else if (!me.eventsSuspended && project) {
        // Instead of making n auto-destroying listeners (which takes enormous amount of time), we make a single
        // one and queue all the events. When dataReady event is triggered we trigger those events
        // https://github.com/bryntum/support/issues/3154
        if (!project.dataReadyDetacher) {
          project.queuedDataReadyEvents = [];
          // Wait for commit without triggering one, otherwise we would affect commit scheduling
          project.dataReadyDetacher = project.ion({
            dataReady() {
              // Trigger "original" event
              this.queuedDataReadyEvents.forEach(([superProto, scope, eventName, param]) => {
                superProto.trigger.call(scope, eventName, param);
              });
              project.queuedDataReadyEvents = null;
              project.dataReadyDetacher();
              project.dataReadyDetacher = null;
            },
            once: true
          });
        }
        project.queuedDataReadyEvents.push([superProto, me, eventName, param]);
      }
      // No way of handling other return values in this scenario, won't work for preventable events
      return true;
    }
    //endregion
    calculateProject() {
      // project is supposed to be provided for stores from outside
      return this.project;
    }
    setStoreData(data) {
      var _this$project;
      // Loading data sets hasLoadedDataToCommit flag.
      // So we treat the 1st commit after data loading as the initial one
      if (this.project && !(this.syncDataOnLoad || this.disableHasLoadedDataToCommitFlag)) {
        this.project.hasLoadedDataToCommit = true;
      }
      this.isLoadingData = true;
      superProto.setStoreData.call(this, data);
      this.isLoadingData = false;
      (_this$project = this.project) === null || _this$project === void 0 ? void 0 : _this$project.trigger('storeRefresh', {
        store: this
      });
    }
    // Override to postpone auto commits to after project commit, makes sure records are unmodified after commit
    async doAutoCommit() {
      if (this.suspendCount <= 0 && this.project && !this.project.isEngineReady()) {
        // @ts-ignore
        await this.project.commitAsync();
      }
      superProto.doAutoCommit.call(this);
    }
    async addAsync(records, silent) {
      const result = this.add(records, silent);
      await this.project.commitAsync();
      return result;
    }
    async insertAsync(index, records, silent) {
      const result = this.insert(index, records, silent);
      await this.project.commitAsync();
      return result;
    }
    async loadDataAsync(data) {
      this.data = data;
      await this.project.commitAsync();
    }
    performFilter() {
      var _this$rootNode;
      if (this.project && (this.isLoadingData || (_this$rootNode = this.rootNode) !== null && _this$rootNode !== void 0 && _this$rootNode.isLoading)) {
        // Reapply filters after calculations, in case filtering on some calculated field
        this.project.commitAsync().then(() => this.filter());
      }
      return super.performFilter(...arguments);
    }
  }
  return AbstractPartOfProjectStoreMixin;
}) {}

/**
 * This an abstract mixin for every Model that belongs to a project.
 *
 * The model with this mixin, supposes that it will be "joining" a store that is already part of a project,
 * so that such model can take a reference to the project from it.
 *
 * It provides 2 template methods [[joinProject]] and [[leaveProject]], which can be overridden in other mixins.
 */
class AbstractPartOfProjectModelMixin extends Mixin([AbstractPartOfProjectGenericMixin, Model], base => {
  const superProto = base.prototype;
  class AbstractPartOfProjectModelMixin extends base {
    joinStore(store) {
      let joinedProject = null;
      // Joining a store that is not part of project (for example a chained store) should not affect engine
      if (isInstanceOf(store, AbstractPartOfProjectStoreMixin)) {
        const project = store.getProject();
        if (project && !this.getProject()) {
          this.setProject(project);
          joinedProject = project;
        }
      }
      superProto.joinStore.call(this, store);
      // Join directly only if not repopulating the store, in which case we will be joined later after
      // graph has been recreated
      if (joinedProject && !joinedProject.isRepopulatingStores) this.joinProject();
    }
    unjoinStore(store, isReplacing = false) {
      superProto.unjoinStore.call(this, store, isReplacing);
      const {
        project
      } = this;
      const isLeavingProjectStore = isInstanceOf(store, AbstractPartOfProjectStoreMixin) && !store.isFillingFromMaster && project === (store.isChained && store.project ? store.masterStore.project : store.project);
      // Leave project when unjoining from store, but do not bother if the project is being destroyed or if
      // the dataset is being replaced, or if store is chained into other project
      if (project && !project.isDestroying && !project.isRepopulatingStores && isLeavingProjectStore) {
        this.leaveProject(isReplacing);
        this.setProject(null);
      }
      // @ts-ignore
      if (isLeavingProjectStore) this.graph = null;
    }
    /**
     * Template method, which is called when model is joining the project (through joining some store that
     * has already joined the project)
     */
    joinProject() {}
    /**
     * Template method, which is called when model is leaving the project (through leaving some store usually)
     */
    leaveProject(isReplacing = false) {}
    calculateProject() {
      const store = this.stores.find(s => isInstanceOf(s, AbstractPartOfProjectStoreMixin) && !!s.getProject());
      return store === null || store === void 0 ? void 0 : store.getProject();
    }
    async setAsync(fieldName, value, silent) {
      var _this$project;
      const result = this.set(fieldName, value, silent);
      await ((_this$project = this.project) === null || _this$project === void 0 ? void 0 : _this$project.commitAsync());
      return result;
    }
    async getAsync(fieldName) {
      var _this$project2;
      await ((_this$project2 = this.project) === null || _this$project2 === void 0 ? void 0 : _this$project2.commitAsync());
      return this.get(fieldName);
    }
    get isStmRestoring() {
      const project = this.getProject();
      return (project === null || project === void 0 ? void 0 : project.isRestoringData) || (project === null || project === void 0 ? void 0 : project.stm.isRestoring) || false;
    }
  }
  return AbstractPartOfProjectModelMixin;
}) {}

/**
 * This is a calendar interval mixin.
 *
 * Can be either a static time interval (if [[startDate]]/[[endDate]] are specified) or recurrent time interval
 * ([[recurrentStartDate]]/[[recurrentEndDate]]).
 *
 * By default it defines a non-working period ([[isWorking]] field has default value `false`),
 * but can also define an explicit working time, for example to override some previous period.
 *
 * You probably don't need to create instances of this mixin directly, instead you pass its configuration object to the [[AbstractCalendarMixin.addInterval]]
 */
class CalendarIntervalMixin extends Mixin([AbstractPartOfProjectModelMixin], base => {
  base.prototype;
  class CalendarIntervalMixin extends base {
    static get fields() {
      return ['name', {
        name: 'startDate',
        type: 'date'
      }, {
        name: 'endDate',
        type: 'date'
      }, 'recurrentStartDate', 'recurrentEndDate', 'cls', 'iconCls', {
        name: 'isWorking',
        type: 'boolean',
        defaultValue: false
      }, {
        name: 'priority',
        type: 'number'
      }];
    }
    getCalendar() {
      return this.stores[0].calendar;
    }
    resetPriority() {
      this.priorityField = null;
      this.getCalendar().getDepth();
    }
    // not just `getPriority` to avoid clash with auto-generated getter in the subclasses
    getPriorityField() {
      if (this.priorityField != null) return this.priorityField;
      // 0 - 10000 interval is reserved for "unspecified time" intervals
      // then 10000 - 10100, 10100-10200, ... etc intervals are for the calendars at depth 0, 1, ... etc
      let base = 10000 + this.getCalendar().getDepth() * 100;
      let priority = this.priority;
      if (priority == null) {
        // recurrent intervals are considered "base" and have lower priority
        // static intervals are considered special case overrides and have higher priority
        priority = this.isRecurrent() ? 20 : 30;
      }
      // intervals from parent calendars will have lower priority
      return this.priorityField = base + priority;
    }
    /**
     * Whether this interval is recurrent (both [[recurrentStartDate]] and [[recurrentEndDate]] are present and parsed correctly
     * by the `later` library)
     */
    isRecurrent() {
      return Boolean(this.recurrentStartDate && this.recurrentEndDate && this.getStartDateSchedule() && this.getEndDateSchedule());
    }
    /**
     * Whether this interval is static - both [[startDate]] and [[endDate]] are present.
     */
    isStatic() {
      return Boolean(this.startDate && this.endDate);
    }
    /**
     * Helper method to parse [[recurrentStartDate]] and [[recurrentEndDate]] field values.
     * @param {Object|String} schedule Recurrence schedule
     * @returns {Object} Processed schedule ready to be used by later.schedule() method.
     * @private
     */
    parseDateSchedule(value) {
      let schedule = value;
      if (value && value !== Object(value)) {
        schedule = later.parse.text(value);
        if (schedule !== Object(schedule) || schedule.error >= 0) {
          // can be provided as JSON text
          try {
            schedule = JSON.parse(value);
          } catch (e) {
            return null;
          }
        }
      }
      return schedule;
    }
    getStartDateSchedule() {
      if (this.startDateSchedule) return this.startDateSchedule;
      const schedule = this.parseDateSchedule(this.recurrentStartDate);
      return this.startDateSchedule = later.schedule(schedule);
    }
    getEndDateSchedule() {
      if (this.endDateSchedule) return this.endDateSchedule;
      if (this.recurrentEndDate === 'EOD') return 'EOD';
      const schedule = this.parseDateSchedule(this.recurrentEndDate);
      return this.endDateSchedule = later.schedule(schedule);
    }
  }
  return CalendarIntervalMixin;
}) {}

/**
 * This a collection of [[CalendarIntervalMixin]] items. Its a dumb collection though, the "real" calendar
 * is a [[AbstractCalendarMixin]] model, which is part of the [[AbstractCalendarManagerStoreMixin]].
 */
class CalendarIntervalStore extends Mixin([AbstractPartOfProjectStoreMixin], base => {
  base.prototype;
  class CalendarIntervalStore extends base {
    constructor() {
      super(...arguments);
      this.disableHasLoadedDataToCommitFlag = true;
    }
    static get defaultConfig() {
      return {
        modelClass: CalendarIntervalMixin
      };
    }
  }
  return CalendarIntervalStore;
}) {}

// Calendar interval model denoting unspecified interval
class UnspecifiedTimeIntervalModel extends Mixin([CalendarIntervalMixin], base => {
  base.prototype;
  class UnspecifiedTimeIntervalModel extends base {
    getCalendar() {
      return this.calendar;
    }
    // NOTE: See parent class implementation for further comments
    getPriorityField() {
      if (this.priorityField != null) return this.priorityField;
      return this.priorityField = this.getCalendar().getDepth();
    }
  }
  return UnspecifiedTimeIntervalModel;
}) {}

/**
 * This a base generic mixin for every class, that belongs to a scheduler_core project.
 *
 * It just provides getter/setter for the `project` property, along with some convenience methods
 * to access the project's stores.
 */
class CorePartOfProjectGenericMixin extends Mixin([AbstractPartOfProjectGenericMixin], base => {
  base.prototype;
  class CorePartOfProjectGenericMixin extends base {
    //region Store getters
    get eventStore() {
      var _this$project;
      return (_this$project = this.project) === null || _this$project === void 0 ? void 0 : _this$project.eventStore;
    }
    get resourceStore() {
      var _this$project2;
      return (_this$project2 = this.project) === null || _this$project2 === void 0 ? void 0 : _this$project2.resourceStore;
    }
    get assignmentStore() {
      var _this$project3;
      return (_this$project3 = this.project) === null || _this$project3 === void 0 ? void 0 : _this$project3.assignmentStore;
    }
    get dependencyStore() {
      var _this$project4;
      return (_this$project4 = this.project) === null || _this$project4 === void 0 ? void 0 : _this$project4.dependencyStore;
    }
    get calendarManagerStore() {
      var _this$project5;
      return (_this$project5 = this.project) === null || _this$project5 === void 0 ? void 0 : _this$project5.calendarManagerStore;
    }
    //endregion
    //region Entity getters
    /**
     * Convenience method to get the instance of event by its id.
     */
    getEventById(id) {
      var _this$eventStore;
      return (_this$eventStore = this.eventStore) === null || _this$eventStore === void 0 ? void 0 : _this$eventStore.getById(id);
    }
    /**
     * Convenience method to get the instance of dependency by its id.
     */
    getDependencyById(id) {
      var _this$dependencyStore;
      return (_this$dependencyStore = this.dependencyStore) === null || _this$dependencyStore === void 0 ? void 0 : _this$dependencyStore.getById(id);
    }
    /**
     * Convenience method to get the instance of resource by its id.
     */
    getResourceById(id) {
      var _this$resourceStore;
      return (_this$resourceStore = this.resourceStore) === null || _this$resourceStore === void 0 ? void 0 : _this$resourceStore.getById(id);
    }
    /**
     * Convenience method to get the instance of assignment by its id.
     */
    getAssignmentById(id) {
      var _this$assignmentStore;
      return (_this$assignmentStore = this.assignmentStore) === null || _this$assignmentStore === void 0 ? void 0 : _this$assignmentStore.getById(id);
    }
    /**
     * Convenience method to get the instance of calendar by its id.
     */
    getCalendarById(id) {
      var _this$calendarManager;
      return (_this$calendarManager = this.calendarManagerStore) === null || _this$calendarManager === void 0 ? void 0 : _this$calendarManager.getById(id);
    }
  }
  return CorePartOfProjectGenericMixin;
}) {}

/**
 * Calendar for project scheduling, mixed by CoreCalendarMixin and BaseCalendarMixin. It is used to mark certain time
 * intervals as "non-working" and ignore them during scheduling.
 *
 * The calendar consists from several [[CalendarIntervalMixin|intervals]]. The intervals can be either static or recurrent.
 */
class AbstractCalendarMixin extends Mixin([AbstractPartOfProjectModelMixin], base => {
  const superProto = base.prototype;
  class CalendarMixin extends base {
    constructor() {
      super(...arguments);
      this.version = 1;
    }
    // intervalStore            : CalendarIntervalStore
    static get fields() {
      return [{
        name: 'version',
        type: 'number'
      }, 'name', {
        name: 'unspecifiedTimeIsWorking',
        type: 'boolean',
        defaultValue: true
      }, {
        name: 'intervals',
        type: 'store',
        subStore: true
      }, 'cls', 'iconCls'];
    }
    get intervalStoreClass() {
      return CalendarIntervalStore;
    }
    get intervalStore() {
      // @ts-ignore
      return this.meta.intervalsStore;
    }
    // Not a typo, name is generated from the fields name = intervals
    initIntervalsStore(config) {
      config.storeClass = this.intervalStoreClass;
      // @ts-ignore
      config.modelClass = this.getDefaultConfiguration().calendarIntervalModelClass || this.intervalStoreClass.defaultConfig.modelClass;
      config.calendar = this;
    }
    // this method is called when the new value for the `intervals` field of this model is assigned
    // the type of the `intervals` field is "store" that's why this magic
    processIntervalsStoreData(intervals) {
      this.bumpVersion();
    }
    isDefault() {
      const project = this.getProject();
      if (project) {
        return this === project.defaultCalendar;
      }
      return false;
    }
    getDepth() {
      return this.childLevel + 1;
    }
    /**
     * The core iterator method of the calendar.
     *
     * @param options The options for iterator. Should contain at least one of the `startDate`/`endDate` properties
     * which indicates what timespan to examine for availability intervals. If one of boundaries is not provided
     * iterator function should return `false` at some point, to avoid infinite loops.
     *
     * Another recognized option is `isForward`, which indicates the direction in which to iterate through the timespan.
     *
     * @param func The iterator function to call. It will be called for every distinct set of availability intervals, found
     * in the given timespan. All the intervals, which are "active" for current interval are collected in the 3rd argument
     * for this function - [[CalendarCacheInterval|calendarCacheInterval]]. If iterator returns `false` (checked with `===`)
     * the iteration stops.
     *
     * @param scope The scope (`this` value) to execute the iterator in.
     */
    forEachAvailabilityInterval(options, func, scope) {
      var _this$getProject;
      const maxRange = (_this$getProject = this.getProject()) === null || _this$getProject === void 0 ? void 0 : _this$getProject.maxCalendarRange;
      if (maxRange) {
        options = Object.assign({
          maxRange
        }, options);
      }
      return this.calendarCache.forEachAvailabilityInterval(options, func, scope);
    }
    /**
     * This method starts at the given `date` and moves forward or backward in time, depending on `isForward`.
     * It stops moving as soon as it accumulates the `durationMs` milliseconds of working time and returns the date
     * at which it has stopped and remaining duration - the [[AccumulateWorkingTimeResult]] object.
     *
     * Normally, the remaining duration will be 0, indicating the full `durationMs` has been accumulated.
     * However, sometimes, calendar might not be able to accumulate enough working time due to various reasons,
     * like if it does not contain enough working time - this case will be indicated with remaining duration bigger than 0.
     *
     * @param date
     * @param durationMs
     * @param isForward
     */
    accumulateWorkingTime(date, durationMs, isForward) {
      var _this$getProject2;
      // if duration is 0 - return the same date
      if (durationMs === 0) return {
        finalDate: new Date(date),
        remainingDurationInMs: 0
      };
      if (isNaN(durationMs)) throw new Error("Invalid duration");
      let finalDate = date;
      const adjustDurationToDST = ((_this$getProject2 = this.getProject()) === null || _this$getProject2 === void 0 ? void 0 : _this$getProject2.adjustDurationToDST) ?? false;
      this.forEachAvailabilityInterval(isForward ? {
        startDate: date,
        isForward: true
      } : {
        endDate: date,
        isForward: false
      }, (intervalStartDate, intervalEndDate, calendarCacheInterval) => {
        let result = true;
        if (calendarCacheInterval.getIsWorking()) {
          let diff = intervalEndDate.getTime() - intervalStartDate.getTime();
          if (durationMs <= diff) {
            if (adjustDurationToDST) {
              const dstDiff = isForward ? intervalStartDate.getTimezoneOffset() - new Date(intervalStartDate.getTime() + durationMs).getTimezoneOffset() : new Date(intervalEndDate.getTime() - durationMs).getTimezoneOffset() - intervalEndDate.getTimezoneOffset();
              durationMs -= dstDiff * 60 * 1000;
            }
            finalDate = isForward ? new Date(intervalStartDate.getTime() + durationMs) : new Date(intervalEndDate.getTime() - durationMs);
            durationMs = 0;
            result = false;
          } else {
            if (adjustDurationToDST) {
              const dstDiff = intervalStartDate.getTimezoneOffset() - intervalEndDate.getTimezoneOffset();
              diff += dstDiff * 60 * 1000;
            }
            finalDate = isForward ? intervalEndDate : intervalStartDate;
            durationMs -= diff;
          }
        }
        return result;
      });
      return {
        finalDate: new Date(finalDate),
        remainingDurationInMs: durationMs
      };
    }
    /**
     * Calculate the working time duration between the 2 dates, in milliseconds.
     *
     * @param {Date} startDate
     * @param {Date} endDate
     * @param {Boolean} [allowNegative] Method ignores negative values by default, returning 0. Set to true to get
     * negative duration.
     */
    calculateDurationMs(startDate, endDate, allowNegative = false) {
      let duration = 0;
      const multiplier = startDate.getTime() <= endDate.getTime() || !allowNegative ? 1 : -1;
      if (multiplier < 0) {
        [startDate, endDate] = [endDate, startDate];
      }
      const adjustDurationToDST = this.getProject().adjustDurationToDST;
      this.forEachAvailabilityInterval({
        startDate: startDate,
        endDate: endDate
      }, (intervalStartDate, intervalEndDate, calendarCacheInterval) => {
        if (calendarCacheInterval.getIsWorking()) {
          duration += intervalEndDate.getTime() - intervalStartDate.getTime();
          if (adjustDurationToDST) {
            const dstDiff = intervalStartDate.getTimezoneOffset() - intervalEndDate.getTimezoneOffset();
            duration += dstDiff * 60 * 1000;
          }
        }
      });
      return duration * multiplier;
    }
    /**
     * Calculate the end date of the time interval which starts at `startDate` and has `durationMs` working time duration
     * (in milliseconds).
     *
     * @param startDate
     * @param durationMs
     */
    calculateEndDate(startDate, durationMs) {
      // the method goes forward by default ..unless a negative duration provided
      const isForward = durationMs >= 0;
      const res = this.accumulateWorkingTime(startDate, Math.abs(durationMs), isForward);
      return res.remainingDurationInMs === 0 ? res.finalDate : null;
    }
    /**
     * Calculate the start date of the time interval which ends at `endDate` and has `durationMs` working time duration
     * (in milliseconds).
     *
     * @param endDate
     * @param durationMs
     */
    calculateStartDate(endDate, durationMs) {
      // the method goes backwards by default ..unless a negative duration provided
      const isForward = durationMs <= 0;
      const res = this.accumulateWorkingTime(endDate, Math.abs(durationMs), isForward);
      return res.remainingDurationInMs === 0 ? res.finalDate : null;
    }
    /**
     * Returns the earliest point at which a working period of time starts, following the given date.
     * Can be the date itself, if it comes on the working time.
     *
     * @param date The date after which to skip the non-working time.
     * @param isForward Whether the "following" means forward in time or backward.
     */
    skipNonWorkingTime(date, isForward = true) {
      let workingDate;
      const res = this.forEachAvailabilityInterval(isForward ? {
        startDate: date,
        isForward: true
      } : {
        endDate: date,
        isForward: false
      }, (intervalStartDate, intervalEndDate, calendarCacheInterval) => {
        if (calendarCacheInterval.getIsWorking()) {
          workingDate = isForward ? intervalStartDate : intervalEndDate;
          return false;
        }
      });
      if (res === CalendarIteratorResult.MaxRangeReached || res === CalendarIteratorResult.FullRangeIterated) return 'empty_calendar';
      return workingDate ? new Date(workingDate) : new Date(date);
    }
    /**
     * This method adds a single [[CalendarIntervalMixin]] to the internal collection of the calendar
     */
    addInterval(interval) {
      return this.addIntervals([interval]);
    }
    /**
     * This method adds an array of [[CalendarIntervalMixin]] to the internal collection of the calendar
     */
    addIntervals(intervals) {
      this.bumpVersion();
      return this.intervalStore.add(intervals);
    }
    /**
     * This method removes a single [[CalendarIntervalMixin]] from the internal collection of the calendar
     */
    removeInterval(interval) {
      return this.removeIntervals([interval]);
    }
    /**
     * This method removes an array of [[CalendarIntervalMixin]] from the internal collection of the calendar
     */
    removeIntervals(intervals) {
      this.bumpVersion();
      return this.intervalStore.remove(intervals);
    }
    /**
     * This method removes all intervals from the internal collection of the calendar
     */
    clearIntervals(silent) {
      if (!silent) {
        this.bumpVersion();
      }
      return this.intervalStore.removeAll(silent);
    }
    bumpVersion() {
      this.clearCache();
      this.version++;
    }
    get calendarCache() {
      if (this.$calendarCache !== undefined) return this.$calendarCache;
      const unspecifiedTimeInterval = new UnspecifiedTimeIntervalModel({
        isWorking: this.unspecifiedTimeIsWorking
      });
      unspecifiedTimeInterval.calendar = this;
      return this.$calendarCache = new CalendarCacheSingle({
        calendar: this,
        unspecifiedTimeInterval: unspecifiedTimeInterval,
        intervalStore: this.intervalStore,
        parentCache: this.parent && !this.parent.isRoot ? this.parent.calendarCache : null
      });
    }
    clearCache() {
      // not strictly needed, we just help garbage collector
      this.$calendarCache && this.$calendarCache.clear();
      this.$calendarCache = undefined;
    }
    resetPriorityOfAllIntervals() {
      this.traverse(calendar => {
        calendar.intervalStore.forEach(interval => interval.resetPriority());
      });
    }
    insertChild(child, before, silent) {
      let res = superProto.insertChild.call(this, ...arguments);
      if (!Array.isArray(res)) {
        res = [res];
      }
      // invalidate cache of the child record, since now it should take parent into account
      res.forEach(r => {
        r.bumpVersion();
        r.resetPriorityOfAllIntervals();
      });
      return res;
    }
    joinProject() {
      superProto.joinProject.call(this);
      this.intervalStore.setProject(this.getProject());
    }
    leaveProject() {
      superProto.leaveProject.call(this);
      this.intervalStore.setProject(null);
      this.clearCache();
    }
    doDestroy() {
      this.leaveProject();
      this.intervalStore.destroy();
      super.doDestroy();
    }
    isDayHoliday(day) {
      const startDate = DateHelper.clearTime(day),
        endDate = DateHelper.getNext(day, TimeUnit.Day);
      let hasWorkingTime = false;
      this.forEachAvailabilityInterval({
        startDate,
        endDate,
        isForward: true
      }, (_intervalStartDate, _intervalEndDate, calendarCacheInterval) => {
        hasWorkingTime = calendarCacheInterval.getIsWorking();
        return !hasWorkingTime;
      });
      return !hasWorkingTime;
    }
    getDailyHolidaysRanges(startDate, endDate) {
      const result = [];
      startDate = DateHelper.clearTime(startDate);
      while (startDate < endDate) {
        if (this.isDayHoliday(startDate)) {
          result.push({
            startDate,
            endDate: DateHelper.getStartOfNextDay(startDate, true, true)
          });
        }
        startDate = DateHelper.getNext(startDate, TimeUnit.Day);
      }
      return result;
    }
    /**
     * Returns working time ranges between the provided dates.
     * @param startDate Start of the period to get ranges from.
     * @param endDate End of the period to get ranges from.
     *
     * @param {Date} startDate
     * @param {Date} endDate
     */
    getWorkingTimeRanges(startDate, endDate) {
      const result = [];
      this.forEachAvailabilityInterval({
        startDate,
        endDate,
        isForward: true
      }, (intervalStartDate, intervalEndDate, calendarCacheInterval) => {
        if (calendarCacheInterval.getIsWorking()) {
          const entry = calendarCacheInterval.intervals[0];
          result.push({
            name: entry.name,
            startDate: intervalStartDate,
            endDate: intervalEndDate
          });
        }
      });
      return result;
    }
    /**
     * Returns non-working time ranges between the provided dates.
     * @param startDate Start of the period to get ranges from.
     * @param endDate End of the period to get ranges from.
     *
     * @param {Date} startDate
     * @param {Date} endDate
     */
    getNonWorkingTimeRanges(startDate, endDate) {
      const result = [];
      this.forEachAvailabilityInterval({
        startDate,
        endDate,
        isForward: true
      }, (intervalStartDate, intervalEndDate, calendarCacheInterval) => {
        if (!calendarCacheInterval.getIsWorking()) {
          const entry = calendarCacheInterval.intervals[0];
          result.push({
            name: entry.name,
            iconCls: entry.iconCls,
            cls: entry.cls,
            startDate: intervalStartDate,
            endDate: intervalEndDate
          });
        }
      });
      return result;
    }
    /**
     * Checks if there is a working time interval in the provided time range (or when just startDate is provided,
     * checks if the date is contained inside a working time interval in this calendar)
     * @param startDate
     * @param [endDate]
     * @param [fullyContained] Pass true to check if the range is fully covered by a single continuous working time block
     */
    isWorkingTime(startDate, endDate, fullyContained) {
      if (fullyContained) {
        let found;
        const res = this.forEachAvailabilityInterval({
          startDate,
          endDate,
          isForward: true
        }, (intervalStartDate, intervalEndDate, calendarCacheInterval) => {
          if (calendarCacheInterval.getIsWorking() && intervalStartDate <= startDate && intervalEndDate >= endDate) {
            found = true;
            return false;
          }
        });
        if (res === CalendarIteratorResult.MaxRangeReached || res === CalendarIteratorResult.FullRangeIterated) return false;
        return found;
      } else {
        // Can be Date | null | 'empty_calendar'
        const workingTimeStart = this.skipNonWorkingTime(startDate);
        return workingTimeStart && workingTimeStart !== 'empty_calendar' ? endDate ? workingTimeStart < endDate : workingTimeStart.getTime() === startDate.getTime() : false;
      }
    }
  }
  return CalendarMixin;
}) {}

/**
 * This is a mixin enabling events to handle assignments. It is mixed by CoreHasAssignmentsMixin and
 * BaseHasAssignmentsMixin. It provides a collection of all assignments, which reference this event.
 *
 * Doesn't affect scheduling.
 */
class AbstractHasAssignmentsMixin extends Mixin([AbstractPartOfProjectModelMixin], base => {
  const superProto = base.prototype;
  class HasAssignmentsMixin extends base {
    /**
     * If a given resource is assigned to this task, returns a [[BaseAssignmentMixin]] instance for it.
     * Otherwise returns `null`
     */
    getAssignmentFor(resource) {
      // Bucket `assigned` might not be set up yet when using delayed calculations
      for (const assignment of this.assigned ?? []) {
        if (assignment.resource === resource) return assignment;
      }
      return null;
    }
    isAssignedTo(resource) {
      return Boolean(this.getAssignmentFor(resource));
    }
    /**
     * A method which assigns a resource to the current event
     */
    async assign(resource) {
      const assignmentCls = this.project.assignmentStore.modelClass;
      this.addAssignment(new assignmentCls({
        event: this,
        resource: resource
      }));
      return this.commitAsync();
    }
    /**
     * A method which unassigns a resource from the current event
     */
    async unassign(resource) {
      const assignment = this.getAssignmentFor(resource);
      this.removeAssignment(assignment);
      return this.commitAsync();
    }
    leaveProject() {
      // `this.assigned` will be empty if model is added to project and then removed immediately
      // w/o any propagations
      // @ts-ignore
      if (this.isInActiveTransaction && this.assigned) {
        const eventStore = this.getEventStore();
        // to batch the assignments removal, we don't remove the assignments right away, but instead
        // add them for the batched removal to the `assignmentsForRemoval` property of the event store
        this.assigned.forEach(assignment => eventStore.assignmentsForRemoval.add(assignment));
      }
      superProto.leaveProject.call(this, ...arguments);
    }
    remove() {
      if (this.parent) {
        // need to get the event store in advance, because after removal the project reference will be cleared (all that is what provide
        // references to all stores
        const eventStore = this.getEventStore();
        superProto.remove.call(this);
        eventStore && eventStore.afterEventRemoval();
      } else {
        return superProto.remove.call(this);
      }
    }
    // template methods, overridden in scheduling modes mixins
    // should probably be named something like "onEventAssignmentAdded"
    // should be a listener for the `add` event of the assignment store instead
    addAssignment(assignment) {
      this.getProject().assignmentStore.add(assignment);
      return assignment;
    }
    // should be a listener for the `remove` event of the assignment store instead
    removeAssignment(assignment) {
      this.getProject().assignmentStore.remove(assignment);
      return assignment;
    }
  }
  return HasAssignmentsMixin;
}) {}

/**
 * This a mixin for every Model that belongs to a scheduler_core project.
 *
 * It adds functions needed to calculate invalidated fields on project commit.
 */
class CorePartOfProjectModelMixin extends Mixin([AbstractPartOfProjectModelMixin, CorePartOfProjectGenericMixin, Model], base => {
  const superProto = base.prototype;
  class CorePartOfProjectModelMixin extends base {
    constructor() {
      super(...arguments);
      // Flag set during calculation
      this.$isCalculating = false;
      // Proposed changes
      this.$changed = {};
      // Value before proposed change, for buckets that need to update data early
      this.$beforeChange = {};
    }
    get isInActiveTransaction() {
      return true;
    }
    // Invalidate record upon joining project, leads to a buffered commit
    joinProject() {
      this.invalidate();
    }
    // Trigger a buffered commit when leaving the project
    leaveProject(isReplacing = false) {
      var _this$project;
      superProto.leaveProject.call(this, isReplacing);
      (_this$project = this.project) === null || _this$project === void 0 ? void 0 : _this$project.bufferedCommitAsync();
    }
    /**
     * Invalidates this record, queueing it for calculation on project commit.
     */
    invalidate() {
      var _this$project2;
      (_this$project2 = this.project) === null || _this$project2 === void 0 ? void 0 : _this$project2.invalidate(this);
    }
    /**
     * Used to retrieve the proposed (before 'dataReady') or current (after 'dataReady') value for a field.
     * If there is no proposed change, it is functionally equal to a normal `record.get()` call.
     */
    getCurrentOrProposed(fieldName) {
      if (fieldName in this.$changed && this.$changed[fieldName] !== true) {
        return this.$changed[fieldName];
      }
      return this.get(fieldName) ?? null;
    }
    /**
     * Determines if the specified field has a value or not, value can be either current or proposed.
     */
    hasCurrentOrProposed(fieldName) {
      return fieldName in this.$changed && this.$changed[fieldName] != true || this.get(fieldName) != null;
    }
    /**
     * Propose changes, to be considered during calculation. Also invalidates the record.
     */
    propose(changes) {
      var _this$recurringTimeSp;
      // @ts-ignore
      if (this.project || (_this$recurringTimeSp = this.recurringTimeSpan) !== null && _this$recurringTimeSp !== void 0 && _this$recurringTimeSp.project) {
        const keys = Object.keys(changes);
        for (let i = 0; i < keys.length; i++) {
          const key = keys[i];
          this.$changed[key] = changes[key];
        }
        this.invalidate();
      } else {
        // If no project, behave as a normal model would
        this.set(changes);
      }
    }
    /**
     * Similar to propose, but with more options. Mostly used by buckets, since they need data to update early.
     */
    setChanged(field, value, invalidate = true, setData = false) {
      const me = this;
      me.$changed[field] = value;
      // Buckets need to keep data up to date immediately
      if (setData) {
        if (!(field in me.$beforeChange)) {
          me.$beforeChange[field] = me.get(field);
        }
        me.setData(field, value);
      }
      invalidate && me.invalidate();
    }
    /**
     * Hook called before project refresh, override and calculate required changes in subclasses
     */
    calculateInvalidated() {}
    /**
     * Called after project refresh, before dataReady. Announce updated data
     */
    finalizeInvalidated(silent = false) {
      const me = this;
      me.$isCalculating = true;
      if (!silent) {
        // First silently revert any data change (used by buckets), otherwise it won't be detected by `set()`
        me.setData(me.$beforeChange);
        // Then do a proper set
        me.set(me.$changed);
      } else {
        me.setData(me.$changed);
      }
      me.$changed = {};
      me.$beforeChange = {};
      me.$isCalculating = false;
    }
  }
  return CorePartOfProjectModelMixin;
}) {}

const isNotNumber = value => Number(value) !== value;
const CIFromSetOrArrayOrValue = value => {
  if (value instanceof Set || value instanceof Array) return CI(value);
  return CI([value]);
};
const delay = value => new Promise(resolve => setTimeout(resolve, value));
const format = (format, ...values) => {
  return format.replace(/{(\d+)}/g, (match, number) => typeof values[number] !== 'undefined' ? values[number] : match);
};

// Shared functionality for CoreAssignmentStore & ChronoAssignmentStore
class AbstractAssignmentStoreMixin extends Mixin([AbstractPartOfProjectStoreMixin], base => {
  const superProto = base.prototype;
  class AbstractAssignmentStoreMixin extends base {
    constructor() {
      super(...arguments);
      this.assignmentsForRemoval = new Set();
      this.allAssignmentsForRemoval = false;
    }
    remove(records, silent) {
      this.assignmentsForRemoval = CIFromSetOrArrayOrValue(records).toSet();
      const res = superProto.remove.call(this, records, silent);
      this.assignmentsForRemoval.clear();
      return res;
    }
    removeAll(silent) {
      this.allAssignmentsForRemoval = true;
      const res = superProto.removeAll.call(this, silent);
      this.allAssignmentsForRemoval = false;
      return res;
    }
  }
  return AbstractAssignmentStoreMixin;
}) {}

/**
 * Shared functionality for [[CoreCalendarManagerStoreMixin]] and [[ChronoCalendarManagerStoreMixin]]
 */
class AbstractCalendarManagerStoreMixin extends Mixin([AbstractPartOfProjectStoreMixin], base => {
  base.prototype;
  class AbstractCalendarManagerStoreMixin extends base {
    // special handling to destroy calendar models as part of destroying this store
    doDestroy() {
      var _this$rootNode;
      const records = [];
      // When chained, traverse can be called on destroyed nodes.
      if (!((_this$rootNode = this.rootNode) !== null && _this$rootNode !== void 0 && _this$rootNode.isDestroyed)) {
        this.traverse(record => records.push(record));
      }
      super.doDestroy();
      records.forEach(record => record.destroy());
    }
  }
  return AbstractCalendarManagerStoreMixin;
}) {}

// Shared functionality for CoreDependencyStore & ChronoDependencyStore
class AbstractDependencyStoreMixin extends Mixin([AbstractPartOfProjectStoreMixin], base => {
  const superProto = base.prototype;
  class AbstractDependencyStoreMixin extends base {
    constructor() {
      super(...arguments);
      this.dependenciesForRemoval = new Set();
      this.allDependenciesForRemoval = false;
    }
    remove(records, silent) {
      this.dependenciesForRemoval = CIFromSetOrArrayOrValue(records).toSet();
      const res = superProto.remove.call(this, records, silent);
      this.dependenciesForRemoval.clear();
      return res;
    }
    removeAll(silent) {
      this.allDependenciesForRemoval = true;
      const res = superProto.removeAll.call(this, silent);
      this.allDependenciesForRemoval = false;
      return res;
    }
  }
  return AbstractDependencyStoreMixin;
}) {}

const dataAddRemoveActions$1 = {
  splice: 1,
  clear: 1
};
// Shared functionality for CoreEventStore & ChronoEventStore
class AbstractEventStoreMixin extends Mixin([AbstractPartOfProjectStoreMixin], base => {
  const superProto = base.prototype;
  class AbstractEventStoreMixin extends base {
    constructor() {
      super(...arguments);
      this.assignmentsForRemoval = new Set();
      this.dependenciesForRemoval = new Set();
    }
    // we need `onDataChange` for `syncDataOnLoad` option to work
    onDataChange(event) {
      var _event$removed;
      // remove from a filter action must be ignored.
      const isAddRemove = dataAddRemoveActions$1[event.action];
      super.onDataChange(event);
      if (isAddRemove && (_event$removed = event.removed) !== null && _event$removed !== void 0 && _event$removed.length) this.afterEventRemoval();
    }
    // it seems `onDataChange` is not triggered for `remove` with `silent` flag
    remove(records, silent) {
      const res = superProto.remove.call(this, records, silent);
      this.afterEventRemoval();
      return res;
    }
    // it seems `onDataChange` is not triggered for `TreeStore#removeAll()`
    removeAll(silent) {
      const res = superProto.removeAll.call(this, silent);
      this.afterEventRemoval();
      return res;
    }
    onNodeRemoveChild(parent, children, index, flags) {
      // @ts-ignore
      const removed = superProto.onNodeRemoveChild.call(this, ...arguments);
      this.afterEventRemoval();
      return removed;
    }
    afterEventRemoval() {
      const {
        assignmentsForRemoval,
        dependenciesForRemoval
      } = this;
      // Can be called from `set data` during construction
      if (!assignmentsForRemoval) return;
      // ORDER IS IMPORTANT!
      // First remove assignments
      const assignmentStore = this.getAssignmentStore();
      if (assignmentStore && !assignmentStore.allAssignmentsForRemoval && assignmentsForRemoval.size) {
        const toRemove = [...assignmentsForRemoval].filter(assignment => !assignmentStore.assignmentsForRemoval.has(assignment));
        toRemove.length > 0 && assignmentStore.remove(toRemove);
      }
      assignmentsForRemoval.clear();
      // Then remove dependencies
      const dependencyStore = this.getDependencyStore();
      if (dependencyStore && !dependencyStore.allDependenciesForRemoval && dependenciesForRemoval.size) {
        const toRemove = [...dependenciesForRemoval].filter(dependency => !dependencyStore.dependenciesForRemoval.has(dependency));
        toRemove.length > 0 && dependencyStore.remove(toRemove);
      }
      dependenciesForRemoval.clear();
    }
    processRecord(eventRecord, isDataset = false) {
      var _this$project;
      if (!((_this$project = this.project) !== null && _this$project !== void 0 && _this$project.isRepopulatingStores)) {
        const existingRecord = this.getById(eventRecord.id);
        const isReplacing = existingRecord && existingRecord !== eventRecord;
        //@ts-ignore
        if (isReplacing && existingRecord.assigned) {
          //@ts-ignore
          for (const assignment of existingRecord.assigned) {
            assignment.event = eventRecord;
          }
        }
      }
      return eventRecord;
    }
  }
  return AbstractEventStoreMixin;
}) {}

const dataAddRemoveActions = {
  splice: 1,
  clear: 1
};
// Shared functionality for CoreResourceStore & ChronoResourceStore
class AbstractResourceStoreMixin extends Mixin([AbstractPartOfProjectStoreMixin], base => {
  const superProto = base.prototype;
  class AbstractResourceStoreMixin extends base {
    constructor() {
      super(...arguments);
      this.assignmentsForRemoval = new Set();
    }
    // we need `onDataChange` for `syncDataOnLoad` option to work
    onDataChange(event) {
      var _event$removed;
      // remove from a filter action must be ignored.
      const isAddRemove = dataAddRemoveActions[event.action];
      super.onDataChange(event);
      if (isAddRemove && (_event$removed = event.removed) !== null && _event$removed !== void 0 && _event$removed.length) this.afterResourceRemoval();
    }
    // it seems `onDataChange` is not triggered for `remove` with `silent` flag
    remove(records, silent) {
      const res = superProto.remove.call(this, records, silent);
      this.afterResourceRemoval();
      return res;
    }
    // it seems `onDataChange` is not triggered for `TreeStore#removeAll()`
    removeAll(silent) {
      const res = superProto.removeAll.call(this, silent);
      this.afterResourceRemoval();
      return res;
    }
    afterResourceRemoval() {
      const assignmentStore = this.getAssignmentStore();
      if (assignmentStore && !assignmentStore.allAssignmentsForRemoval) {
        const assignmentsForRemoval = [...this.assignmentsForRemoval].filter(assignment => !assignmentStore.assignmentsForRemoval.has(assignment));
        assignmentsForRemoval.length > 0 && assignmentStore.remove(assignmentsForRemoval);
      }
      this.assignmentsForRemoval.clear();
    }
    processRecord(resourceRecord, isDataset = false) {
      const existingRecord = this.getById(resourceRecord.id);
      const isReplacing = existingRecord && existingRecord !== resourceRecord;
      if (isReplacing) {
        //@ts-ignore
        for (const assignment of existingRecord.assigned || []) {
          assignment.resource = resourceRecord;
        }
      }
      return resourceRecord;
    }
  }
  return AbstractResourceStoreMixin;
}) {}

/**
 * Core event entity mixin type.
 *
 * At this level event is only aware about its dates
 * The functionality, related to the assignments etc is provided in other mixins.
 */
class CoreEventMixin extends Mixin([CorePartOfProjectModelMixin], base => {
  const superProto = base.prototype;
  class CoreEventMixin extends base {
    constructor() {
      super(...arguments);
      this._startDate = null;
      this._endDate = null;
      this._duration = null;
    }
    // Proper engine defines these fields since they enter graph, thus we need them
    static get fields() {
      return [{
        name: 'startDate',
        type: 'date'
      }, {
        name: 'endDate',
        type: 'date'
      }, {
        name: 'duration',
        type: 'number'
      }, {
        name: 'durationUnit',
        type: 'durationunit',
        defaultValue: 'day'
      }];
    }
    // Getters return current or proposed value
    get startDate() {
      return this._startDate ?? this.get('startDate') ?? null;
    }
    get endDate() {
      return this._endDate ?? this.get('endDate') ?? null;
    }
    get duration() {
      return this._duration ?? this.get('duration') ?? null;
    }
    // Route all setting through applyXX (setStartDate, startDate = , set('startDate'), batching)
    set startDate(value) {
      this.proposeStartDate(value);
    }
    set endDate(value) {
      this.proposeEndDate(value);
    }
    set duration(value) {
      this.proposeDuration(value);
    }
    //region Edge case normalization
    inSet(field, value, silent, fromRelationUpdate, skipAccessors) {
      const me = this;
      // Catch "normal" set calls
      if (me.project && !me.project.isWritingData && typeof field !== 'string' && !skipAccessors) {
        // Flag fields as changed even if we are given a no-change value, to let calculations take decision on
        // whether to keep duration or not
        if ('startDate' in field && !('startDate' in me.$changed)) {
          me.$changed.startDate = true;
          me.invalidate();
        }
        if ('endDate' in field && !('endDate' in me.$changed)) {
          me.$changed.endDate = true;
          me.invalidate();
        }
      }
      // @ts-ignore
      return superProto.inSet.call(me, field, value, silent, fromRelationUpdate, skipAccessors);
    }
    //endregion
    //region StartDate
    getStartDate() {
      return this.startDate;
    }
    proposeStartDate(startDate, keepDuration = !('endDate' in this.$changed)) {
      var _this$startDate;
      if (this.inSetting || (startDate === null || startDate === void 0 ? void 0 : startDate.getTime()) !== ((_this$startDate = this.startDate) === null || _this$startDate === void 0 ? void 0 : _this$startDate.getTime())) {
        this._startDate = startDate;
        this.propose({
          startDate,
          keepDuration
        });
      }
    }
    async setStartDate(startDate, keepDuration = true) {
      var _this$project;
      this.proposeStartDate(startDate, keepDuration);
      return (_this$project = this.project) === null || _this$project === void 0 ? void 0 : _this$project.commitAsync();
    }
    //endregion
    //region EndDate
    getEndDate() {
      return this.endDate;
    }
    proposeEndDate(endDate, keepDuration = false) {
      var _this$endDate;
      if (this.inSetting || (endDate === null || endDate === void 0 ? void 0 : endDate.getTime()) !== ((_this$endDate = this.endDate) === null || _this$endDate === void 0 ? void 0 : _this$endDate.getTime())) {
        this._endDate = endDate;
        this.propose({
          endDate,
          keepDuration
        });
      }
    }
    async setEndDate(endDate, keepDuration = false) {
      var _this$project2;
      this.proposeEndDate(endDate, keepDuration);
      return (_this$project2 = this.project) === null || _this$project2 === void 0 ? void 0 : _this$project2.commitAsync();
    }
    //endregion
    //region Duration
    getDuration() {
      return this.duration;
    }
    proposeDuration(duration, unit, keepStart = true) {
      this._duration = duration;
      this.propose({
        duration,
        keepStart
      });
      if (unit) this.propose({
        durationUnit: unit
      });
    }
    async setDuration(duration, unit, keepStart = true) {
      var _this$project3;
      this.proposeDuration(duration, unit, keepStart);
      return (_this$project3 = this.project) === null || _this$project3 === void 0 ? void 0 : _this$project3.commitAsync();
    }
    getDurationUnit() {
      return this.durationUnit;
    }
    //endregion
    // When joining as part of inline data, store is available. If joining through load, it is passed
    joinProject() {
      const me = this;
      const changed = me.$changed;
      const startDate = me.getCurrentOrProposed('startDate');
      const endDate = me.getCurrentOrProposed('endDate');
      const duration = me.getCurrentOrProposed('duration');
      // Initial values should be considered changed, to be normalized
      if (startDate != null) changed.startDate = me._startDate = startDate;
      if (endDate != null) changed.endDate = me._endDate = endDate;
      if (duration != null) changed.duration = me._duration = duration;
      // Resolve assignments when event joins project after load
      if (me.eventStore && !me.eventStore.isLoadingData) {
        var _me$assignmentStore;
        const unresolved = (_me$assignmentStore = me.assignmentStore) === null || _me$assignmentStore === void 0 ? void 0 : _me$assignmentStore.storage.findItem('event', null);
        if (unresolved) {
          var _me$assignmentStore2;
          // To avoid n² iterations over raw assignments we cache them by raw eventId, which saves us
          // some iterations over the storage
          // https://github.com/bryntum/support/issues/3141
          const cachedAssignments = (_me$assignmentStore2 = me.assignmentStore) === null || _me$assignmentStore2 === void 0 ? void 0 : _me$assignmentStore2.storage.findItem('eventId', me.id);
          if (cachedAssignments) {
            for (const assignment of cachedAssignments) {
              assignment.setChanged('event', me);
            }
          } else {
            for (const assignment of unresolved) {
              if (assignment.getCurrentOrProposed('event') === me.id) {
                assignment.setChanged('event', me);
              }
            }
          }
        }
      }
      superProto.joinProject.call(me);
    }
    // Mimic how proper engine applies values
    applyValue(useProp, key, value, skipAccessors, field) {
      var _this$recurringTimeSp;
      // @ts-ignore
      if ((this.project || (_this$recurringTimeSp = this.recurringTimeSpan) !== null && _this$recurringTimeSp !== void 0 && _this$recurringTimeSp.project) && field) {
        const {
          name
        } = field;
        if (name === 'startDate' || name == 'duration' || name === 'endDate') {
          useProp = true;
          // Update cached value
          this['_' + name] = value;
        }
        if (skipAccessors) {
          useProp = false;
        }
      }
      superProto.applyValue.call(this, useProp, key, value, skipAccessors, field);
    }
    // Catch changes from batches etc. In which case it is sometimes expected for data to be available directly
    afterChange(toSet, wasSet, silent, fromRelationUpdate, skipAccessors) {
      if (!this.$isCalculating && !skipAccessors) {
        // In certain scenarios data is expected to be available of the bat, messy!
        this.setData({
          startDate: this.getCurrentOrProposed('startDate'),
          endDate: this.getCurrentOrProposed('endDate'),
          duration: this.getCurrentOrProposed('duration'),
          durationUnit: this.getCurrentOrProposed('durationUnit')
        });
      }
      superProto.afterChange.call(this, toSet, wasSet, silent, fromRelationUpdate, skipAccessors);
    }
    // Normalizes dates & duration
    calculateInvalidated() {
      const me = this;
      const changed = me.$changed;
      const changedStart = ('startDate' in changed);
      const changedEnd = ('endDate' in changed);
      const changedDuration = ('duration' in changed);
      const {
        startDate,
        endDate,
        duration,
        keepDuration,
        keepStart
      } = changed;
      let calculate = null;
      // Only start changed
      if (changedStart && !changedEnd && !changedDuration) {
        // Also null end when nulling start (keeping duration)
        if (startDate === null) {
          changed.endDate = null;
        }
        // Start after end without keeping duration -> move end to start
        else if (me.hasCurrentOrProposed('endDate') && startDate > me.getCurrentOrProposed('endDate') && !keepDuration) {
          changed.endDate = startDate;
          changed.duration = 0;
        }
        // Start changed and we either have a duration that we want to keep or no end -> calculate end
        else if (me.hasCurrentOrProposed('duration') && (keepDuration || !me.hasCurrentOrProposed('endDate'))) {
          calculate = 'endDate';
        }
        // Start change and we have an end already -> calculate duration
        else if (me.hasCurrentOrProposed('endDate')) {
          calculate = 'duration';
        }
      }
      // Only end changed
      else if (!changedStart && changedEnd && !changedDuration) {
        // Also null start when nulling end (keeping duration)
        if (endDate === null) {
          changed.startDate = null;
        }
        // End before start without keeping duration -> move start to end
        else if (me.hasCurrentOrProposed('startDate') && !keepDuration && endDate !== true && endDate < me.getCurrentOrProposed('startDate')) {
          changed.startDate = endDate;
          changed.duration = 0;
        }
        // End changed and we either have a duration that we want to keep or no start -> calculate start
        else if (me.hasCurrentOrProposed('duration') && (keepDuration || !me.hasCurrentOrProposed('startDate'))) {
          calculate = 'startDate';
        }
        // End changed and we have a start already -> calculate duration
        else if (me.hasCurrentOrProposed('startDate')) {
          calculate = 'duration';
        }
      }
      // Only duration changed
      else if (!changedStart && !changedEnd && changedDuration) {
        // Also null end when nulling duration (keeping start)
        if (duration === null) {
          changed.endDate = null;
        }
        // Duration changed and we either have a start that we want to keep or no end -> calculate end
        else if (me.hasCurrentOrProposed('startDate') && (keepStart || !me.hasCurrentOrProposed('endDate'))) {
          if (keepStart && changed.duration < 0) {
            changed.duration = 0;
          }
          calculate = 'endDate';
        }
        // Duration changed and we have an end already -> calculate start
        else if (me.hasCurrentOrProposed('endDate')) {
          calculate = 'startDate';
        }
      }
      // Start and end change, affect duration
      else if (changedStart && changedEnd && !changedDuration) {
        // Both nulled, null duration
        if (startDate === null && endDate === null) {
          changed.duration = null;
        }
        // Other cases -> calculate duration
        else {
          calculate = 'duration';
        }
      }
      // Start and duration change -> calculate end
      else if (changedStart && !changedEnd && changedDuration) {
        calculate = 'endDate';
      }
      // End and duration changed -> calculate start
      else if (!changedStart && changedEnd && changedDuration) {
        calculate = 'startDate';
      }
      // All changed -> calculate whichever is null or by default end to be sure things add up
      else if (changedStart && changedEnd && changedDuration) {
        if (duration == null) {
          calculate = 'duration';
        } else if (startDate == null) {
          calculate = 'startDate';
        } else {
          calculate = 'endDate';
        }
      }
      // Normalize if needed
      const currentOrProposedStartDate = me.getCurrentOrProposed('startDate');
      const currentOrProposedEndDate = me.getCurrentOrProposed('endDate');
      const currentOrProposedDuration = me.getCurrentOrProposed('duration');
      const currentOrProposedDurationUnit = me.getCurrentOrProposed('durationUnit');
      let hourDuration, targetDuration;
      switch (calculate) {
        case 'startDate':
          changed.startDate = DateHelper.add(currentOrProposedEndDate, -currentOrProposedDuration, currentOrProposedDurationUnit);
          break;
        case 'endDate':
          // convert proposed duration to hours to safely add over DST
          hourDuration = DateHelper.as('hour', currentOrProposedDuration, currentOrProposedDurationUnit);
          // convert calculated duration to task duration in task duration unit
          targetDuration = DateHelper.as(currentOrProposedDurationUnit, hourDuration, 'h');
          changed.endDate = DateHelper.add(currentOrProposedStartDate, targetDuration, currentOrProposedDurationUnit);
          break;
        case 'duration':
          // convert proposed duration to hours to safely add over DST
          hourDuration = DateHelper.diff(currentOrProposedStartDate, currentOrProposedEndDate, 'h');
          // convert calculated duration to task duration in task duration unit
          changed.duration = DateHelper.as(currentOrProposedDurationUnit, hourDuration, 'h');
          break;
      }
      if ('startDate' in changed && changed.startDate !== true) this._startDate = changed.startDate;
      if ('endDate' in changed && changed.endDate !== true) this._endDate = changed.endDate;
      if ('duration' in changed && changed.duration !== true) this._duration = changed.duration;
      // Clean up flags, not intended to be actually set
      if (changed.startDate === true) delete changed.startDate;
      if (changed.endDate === true) delete changed.endDate;
      delete changed.keepDuration;
      delete changed.keepStart;
    }
  }
  return CoreEventMixin;
}) {}

/**
 * Core resource model class.
 */
class CoreResourceMixin extends Mixin([CorePartOfProjectModelMixin], base => {
  const superProto = base.prototype;
  class CoreResourceMixin extends base {
    get assigned() {
      var _this$project;
      return (_this$project = this.project) === null || _this$project === void 0 ? void 0 : _this$project.assignmentStore.getResourcesAssignments(this);
    }
    joinProject() {
      // Set up assignment -> resource mapping when joining store after assignment (skip during load)
      // (note that there is no resourceStore yet when loading inline data, thus the first part of the condition)
      if (this.resourceStore && !this.resourceStore.isLoadingData) {
        var _this$assignmentStore;
        (_this$assignmentStore = this.assignmentStore) === null || _this$assignmentStore === void 0 ? void 0 : _this$assignmentStore.query(a => a.get('resource') === this.id).forEach(unresolved => unresolved.setChanged('resource', this));
      }
      superProto.joinProject.call(this);
    }
    leaveProject(isReplacing = false) {
      var _this$resourceStore;
      // `this.assigned` will be empty if model is added to project and then removed immediately
      // w/o any propagations
      // when replacing a resource or reloading the store, the assignments should be left intact
      if (this.assigned && !isReplacing && !((_this$resourceStore = this.resourceStore) !== null && _this$resourceStore !== void 0 && _this$resourceStore.isLoadingData)) {
        const resourceStore = this.resourceStore;
        // to batch the assignments removal, we don't remove the assignments right away, but instead
        // add them for the batched removal to the `assignmentsForRemoval` property of the event store
        this.assigned.forEach(assignment => resourceStore.assignmentsForRemoval.add(assignment));
      }
      superProto.leaveProject.call(this);
    }
    applyValue(useProp, key, value, skipAccessor, field) {
      // Changing id on a resource should update resourceId on its assignments
      // (note that field might not exist, if user supplies data for undefined fields)
      if ((field === null || field === void 0 ? void 0 : field.name) === 'id') {
        this.assigned.forEach(assignment => {
          assignment.set('resourceId', value);
        });
      }
      superProto.applyValue.call(this, useProp, key, value, skipAccessor, field);
    }
  }
  return CoreResourceMixin;
}) {}

function asId(recordOrId) {
  return recordOrId !== null && recordOrId !== void 0 && recordOrId.isModel ? recordOrId.id : recordOrId;
}
/**
 * Core assignment model class. It just contains references to the [[CoreEventMixin|event]] and [[CoreResourceMixin|resource]] being assigned.
 */
class CoreAssignmentMixin extends Mixin([CorePartOfProjectModelMixin], base => {
  const superProto = base.prototype;
  class CoreAssignmentMixin extends base {
    // Fields declared in the Model way, existing decorators all assume ChronoGraph is used
    static get fields() {
      return [
      // isEqual required to properly detect changed resource / event
      {
        name: 'resource',
        isEqual: (a, b) => a === b,
        persist: false
      }, {
        name: 'event',
        isEqual: (a, b) => a === b,
        persist: false
      }];
    }
    // Resolve early + update indices to have buckets ready before commit
    setChanged(field, value, invalidate) {
      const {
        assignmentStore,
        eventStore,
        resourceStore,
        project
      } = this;
      let update = false;
      if (field === 'event') {
        const event = isInstanceOf(value, CoreEventMixin) ? value : eventStore === null || eventStore === void 0 ? void 0 : eventStore.$master.getById(value);
        if (event) update = true;
        value = event || value;
      }
      if (field === 'resource') {
        const resource = isInstanceOf(value, CoreResourceMixin) ? value : resourceStore === null || resourceStore === void 0 ? void 0 : resourceStore.$master.getById(value);
        if (resource) update = true;
        value = resource || value;
      }
      // Passing true as last arg, bucket expected to work before commit
      superProto.setChanged.call(this, field, value, invalidate, true);
      // Update on resolve, if this is a single operation and record is part of project (might be standalone record)
      if (assignmentStore && update && !project.isPerformingCommit && !assignmentStore.isLoadingData && !resourceStore.isLoadingData && !assignmentStore.skipInvalidateIndices) {
        assignmentStore.invalidateIndices();
      }
    }
    // Resolve event and resource when joining project
    joinProject() {
      superProto.joinProject.call(this);
      this.setChanged('event', this.get('event'));
      this.setChanged('resource', this.get('resource'));
    }
    // Resolved resource & event as part of commit
    // Normally done earlier in setChanged, but stores might not have been available yet at that point
    calculateInvalidated() {
      // Changed values, should be used instead of current where available
      let {
        event = this.event,
        resource = this.resource
      } = this.$changed;
      if (event !== null && !isInstanceOf(event, CoreEventMixin)) {
        var _this$eventStore;
        const resolved = (_this$eventStore = this.eventStore) === null || _this$eventStore === void 0 ? void 0 : _this$eventStore.getById(event);
        if (resolved) this.setChanged('event', resolved, false);
      }
      if (resource !== null && !isInstanceOf(resource, CoreResourceMixin)) {
        var _this$resourceStore;
        const resolved = (_this$resourceStore = this.resourceStore) === null || _this$resourceStore === void 0 ? void 0 : _this$resourceStore.getById(resource);
        if (resolved) this.setChanged('resource', resolved, false);
      }
    }
    // resourceId and eventId required to be available for new datasets
    finalizeInvalidated(silent) {
      const changed = this.$changed;
      if ('resource' in changed) {
        changed.resourceId = asId(changed.resource);
      }
      if ('event' in changed) {
        changed.eventId = asId(changed.event);
      }
      superProto.finalizeInvalidated.call(this, silent);
    }
    //region Event
    set event(event) {
      this.setChanged('event', event);
      this.setChanged('eventId', asId(event));
    }
    get event() {
      const event = this.get('event');
      // Engine returns null instead of id when unresolved
      return (event === null || event === void 0 ? void 0 : event.id) != null ? event : null;
    }
    //endregion
    //region Resource
    set resource(resource) {
      this.setChanged('resource', resource);
      this.setChanged('resourceId', asId(resource));
    }
    get resource() {
      const resource = this.get('resource');
      // Engine returns null instead of id when unresolved
      return (resource === null || resource === void 0 ? void 0 : resource.id) != null ? resource : null;
    }
  }
  return CoreAssignmentMixin;
}) {}

/**
 * This a mixin for every Store, that belongs to a scheduler_core project.
 */
class CorePartOfProjectStoreMixin extends Mixin([AbstractPartOfProjectStoreMixin, CorePartOfProjectGenericMixin, Store], base => {
  const superProto = base.prototype;
  class CorePartOfProjectStoreMixin extends base {
    setProject(project) {
      const result = superProto.setProject.call(this, project);
      if (project) this.joinProject(project);
      return result;
    }
    joinProject(project) {}
    onCommitAsync() {}
  }
  return CorePartOfProjectStoreMixin;
}) {}

const emptySet$1 = new Set();
/**
 * A store mixin class, that represent collection of all assignments in the [[SchedulerCoreProjectMixin|project]].
 */
class CoreAssignmentStoreMixin extends Mixin([AbstractAssignmentStoreMixin, CorePartOfProjectStoreMixin], base => {
  base.prototype;
  class CoreAssignmentStoreMixin extends base {
    constructor() {
      super(...arguments);
      this.skipInvalidateIndices = false;
    }
    static get defaultConfig() {
      return {
        modelClass: CoreAssignmentMixin,
        storage: {
          extraKeys: [{
            property: 'event',
            unique: false
          }, {
            property: 'resource',
            unique: false
          }, {
            property: 'eventId',
            unique: false
          }]
        }
      };
    }
    set data(value) {
      this.allAssignmentsForRemoval = true;
      super.data = value;
      this.allAssignmentsForRemoval = false;
    }
    getEventsAssignments(event) {
      return this.storage.findItem('event', event, true) || emptySet$1;
    }
    getResourcesAssignments(resource) {
      return this.storage.findItem('resource', resource.$original, true) || emptySet$1;
    }
    updateIndices() {
      this.storage.rebuildIndices();
    }
    invalidateIndices() {
      this.storage.invalidateIndices();
    }
    afterLoadData() {
      this.eventStore && this.linkAssignments(this.eventStore, 'event');
      this.resourceStore && this.linkAssignments(this.resourceStore, 'resource');
    }
    // Link events/resources to assignments, called when those stores are populated or joined to project
    linkAssignments(store, modelName) {
      // If we are passed a chained store, the ultimate source of truth is the masterStore
      store = store.masterStore || store;
      const unresolved = this.count && this.storage.findItem(modelName, null, true);
      if (unresolved) {
        for (const assignment of unresolved) {
          const record = store.getById(assignment.getCurrentOrProposed(modelName));
          if (record) assignment.setChanged(modelName, record);
        }
        this.invalidateIndices();
      }
    }
    // Unlink events/resources from assignments, called when those stores are cleared
    unlinkAssignments(modelName) {
      // Invalidate links to events/resources, need to link to new records so set it back to the id (might be resource or resourceId)
      // As assignment.resource returns null if it's an id, need to check for that in data
      this.forEach(assignment => {
        var _assignment$modelName;
        return assignment.setChanged(modelName, ((_assignment$modelName = assignment[modelName]) === null || _assignment$modelName === void 0 ? void 0 : _assignment$modelName.id) ?? (assignment === null || assignment === void 0 ? void 0 : assignment.getData(modelName)) ?? assignment[modelName + 'Id']);
      });
      this.invalidateIndices();
    }
    onCommitAsync() {
      this.updateIndices();
    }
  }
  return CoreAssignmentStoreMixin;
}) {}

/**
 * The calendar for project scheduling, it is used to mark certain time intervals as "non-working" and ignore them during scheduling.
 *
 * The calendar consists from several [[CalendarIntervalMixin|intervals]]. The intervals can be either static or recurrent.
 */
class CoreCalendarMixin extends Mixin([AbstractCalendarMixin, CorePartOfProjectModelMixin], base => {
  base.prototype;
  class CoreCalendarMixin extends base {}
  return CoreCalendarMixin;
}) {}

/**
 * A store mixin class, that represent collection of all calendars in the [[SchedulerCoreProjectMixin|project]].
 */
class CoreCalendarManagerStoreMixin extends Mixin([AbstractCalendarManagerStoreMixin, CorePartOfProjectStoreMixin], base => {
  base.prototype;
  class CoreCalendarManagerStoreMixin extends base {
    static get defaultConfig() {
      return {
        tree: true,
        modelClass: CoreCalendarMixin
      };
    }
  }
  return CoreCalendarManagerStoreMixin;
}) {}

class CoreDependencyMixin extends Mixin([CorePartOfProjectModelMixin], base => {
  const superProto = base.prototype;
  class CoreDependencyMixin extends base {
    static get fields() {
      return [{
        name: 'fromEvent',
        isEqual: (a, b) => a === b,
        persist: false
      }, {
        name: 'toEvent',
        isEqual: (a, b) => a === b,
        persist: false
      }];
    }
    // Resolve early + update indices to have buckets ready before commit
    setChanged(field, value, invalidate) {
      var _this$dependencyStore;
      let update = false;
      if (field === 'fromEvent' || field === 'toEvent') {
        var _this$eventStore;
        const event = isInstanceOf(value, CoreEventMixin) ? value : (_this$eventStore = this.eventStore) === null || _this$eventStore === void 0 ? void 0 : _this$eventStore.getById(value);
        if (event) update = true;
        value = event || value;
      }
      superProto.setChanged.call(this, field, value, invalidate, true);
      if (update && !this.project.isPerformingCommit && !((_this$dependencyStore = this.dependencyStore) !== null && _this$dependencyStore !== void 0 && _this$dependencyStore.isLoadingData)) {
        var _this$dependencyStore2;
        (_this$dependencyStore2 = this.dependencyStore) === null || _this$dependencyStore2 === void 0 ? void 0 : _this$dependencyStore2.invalidateIndices();
      }
    }
    // Resolve events when joining project
    joinProject() {
      superProto.joinProject.call(this);
      // Initial values should be considered changed, to be normalized
      // (needs to pass through setChanged for early normalization expected for buckets)
      this.setChanged('fromEvent', this.get('fromEvent'));
      this.setChanged('toEvent', this.get('toEvent'));
    }
    // Resolved events as part of commit
    // Normally done earlier in setChanged, but stores might not have been available yet at that point
    calculateInvalidated() {
      // Changed values, should be used instead of current where available
      let {
        fromEvent,
        toEvent
      } = this.$changed;
      if (fromEvent !== null && !isInstanceOf(fromEvent, CoreEventMixin)) {
        var _this$eventStore2;
        const resolved = (_this$eventStore2 = this.eventStore) === null || _this$eventStore2 === void 0 ? void 0 : _this$eventStore2.getById(fromEvent);
        if (resolved) this.$changed.fromEvent = resolved;
      }
      if (toEvent !== null && !isInstanceOf(toEvent, CoreEventMixin)) {
        var _this$eventStore3;
        const resolved = (_this$eventStore3 = this.eventStore) === null || _this$eventStore3 === void 0 ? void 0 : _this$eventStore3.getById(toEvent);
        if (resolved) this.$changed.toEvent = resolved;
      }
    }
    //region Events
    // Not using "propose" mechanism from CoreEventMixin, because buckets are expected to be up to date right away
    set fromEvent(fromEvent) {
      this.setChanged('fromEvent', fromEvent);
    }
    get fromEvent() {
      const fromEvent = this.get('fromEvent');
      // Engine returns null instead of id when unresolved
      return (fromEvent === null || fromEvent === void 0 ? void 0 : fromEvent.id) != null ? fromEvent : null;
    }
    set toEvent(toEvent) {
      this.setChanged('toEvent', toEvent);
    }
    get toEvent() {
      const toEvent = this.get('toEvent');
      // Engine returns null instead of id when unresolved
      return (toEvent === null || toEvent === void 0 ? void 0 : toEvent.id) != null ? toEvent : null;
    }
  }
  return CoreDependencyMixin;
}) {}

const emptySet = new Set();
/**
 * A store mixin class, that represent collection of all dependencies in the [[SchedulerCoreProjectMixin|project]].
 */
class CoreDependencyStoreMixin extends Mixin([AbstractDependencyStoreMixin, CorePartOfProjectStoreMixin], base => {
  base.prototype;
  class CoreDependencyStoreMixin extends base {
    constructor() {
      super(...arguments);
      this.dependenciesForRemoval = new Set();
      this.allDependenciesForRemoval = false;
    }
    static get defaultConfig() {
      return {
        modelClass: CoreDependencyMixin,
        storage: {
          extraKeys: [{
            property: 'fromEvent',
            unique: false
          }, {
            property: 'toEvent',
            unique: false
          }]
        }
      };
    }
    getIncomingDepsForEvent(event) {
      return this.storage.findItem('toEvent', event) || emptySet;
    }
    getOutgoingDepsForEvent(event) {
      return this.storage.findItem('fromEvent', event) || emptySet;
    }
    set data(value) {
      this.allDependenciesForRemoval = true;
      super.data = value;
      this.allDependenciesForRemoval = false;
    }
    updateIndices() {
      this.storage.rebuildIndices();
    }
    invalidateIndices() {
      this.storage.invalidateIndices();
    }
    onCommitAsync() {
      this.updateIndices();
    }
  }
  return CoreDependencyStoreMixin;
}) {}

/**
 * This is a mixin, which can be applied to the [[CoreEventMixin]]. It provides the collection of all assignments,
 * which reference this event.
 *
 * Doesn't affect scheduling.
 */
class CoreHasAssignmentsMixin extends Mixin([CoreEventMixin, AbstractHasAssignmentsMixin], base => {
  const superProto = base.prototype;
  class CoreHasAssignmentsMixin extends base {
    get assigned() {
      var _this$project;
      return ((_this$project = this.project) === null || _this$project === void 0 ? void 0 : _this$project.assignmentStore.getEventsAssignments(this)) ?? this.$cachedAssignments;
    }
    leaveProject(isReplacing = false) {
      // Expected to still be able to return assignments when removed from project
      this.$cachedAssignments = this.assigned;
      super.leaveProject(isReplacing);
    }
    applyValue(useProp, key, value, skipAccessor, field) {
      // Changing id on an event should update resourceId on its assignments
      if (key === 'id') {
        var _this$assigned;
        (_this$assigned = this.assigned) === null || _this$assigned === void 0 ? void 0 : _this$assigned.forEach(assignment => assignment.set('eventId', value));
      }
      superProto.applyValue.call(this, useProp, key, value, skipAccessor, field);
    }
    copy(newId = null, deep = null) {
      const copy = superProto.copy.call(this, newId, deep);
      // If deep is everything but object - use default behavior, which is to invoke accessors
      // If deep is an object, check if it has certain field disabled
      if (ObjectHelper.isObject(deep) && !deep.skipFieldIdentifiers || !ObjectHelper.isObject(deep)) {
        // Copy current assignments, used for occurrences
        copy.$cachedAssignments = this.assigned;
      }
      return copy;
    }
  }
  return CoreHasAssignmentsMixin;
}) {}

/**
 * This is a mixin, which can be applied to the [[CoreEventMixin]]. It provides the collection of all dependencies,
 * which reference this event.
 *
 * Doesn't affect scheduling.
 */
class CoreHasDependenciesMixin extends Mixin([CoreEventMixin], base => {
  const superProto = base.prototype;
  class CoreHasDependenciesMixin extends base {
    get outgoingDeps() {
      return this.project.dependencyStore.getOutgoingDepsForEvent(this);
    }
    get incomingDeps() {
      return this.project.dependencyStore.getIncomingDepsForEvent(this);
    }
    leaveProject() {
      const eventStore = this.eventStore;
      // the buckets may be empty if a model is removed from the project immediately after adding
      // (without propagation)
      if (this.outgoingDeps) {
        this.outgoingDeps.forEach(dependency => eventStore.dependenciesForRemoval.add(dependency));
      }
      if (this.incomingDeps) {
        this.incomingDeps.forEach(dependency => eventStore.dependenciesForRemoval.add(dependency));
      }
      superProto.leaveProject.call(this);
    }
  }
  return CoreHasDependenciesMixin;
}) {}

/**
 * This is an event class, [[SchedulerCoreProjectMixin]] is working with.
 * It is constructed as [[CoreEventMixin]], enhanced with [[CoreHasAssignmentsMixin]] and [[CoreHasDependenciesMixin]]
 */
class SchedulerCoreEvent extends Mixin([CoreEventMixin, CoreHasAssignmentsMixin, CoreHasDependenciesMixin], base => {
  base.prototype;
  class SchedulerCoreEvent extends base {}
  return SchedulerCoreEvent;
}) {}

/**
 * A store mixin class, that represent collection of all events in the [[SchedulerCoreProjectMixin|project]].
 */
class CoreEventStoreMixin extends Mixin([AbstractEventStoreMixin, CorePartOfProjectStoreMixin], base => {
  base.prototype;
  class CoreEventStoreMixin extends base {
    static get defaultConfig() {
      return {
        modelClass: SchedulerCoreEvent
      };
    }
    joinProject() {
      var _this$assignmentStore;
      (_this$assignmentStore = this.assignmentStore) === null || _this$assignmentStore === void 0 ? void 0 : _this$assignmentStore.linkAssignments(this, 'event');
    }
    afterLoadData() {
      var _this$assignmentStore2;
      this.afterEventRemoval();
      (_this$assignmentStore2 = this.assignmentStore) === null || _this$assignmentStore2 === void 0 ? void 0 : _this$assignmentStore2.linkAssignments(this, 'event');
    }
  }
  return CoreEventStoreMixin;
}) {}
// /**
//  * The tree store version of [[EventStoreMixin]].
//  */
// export class EventTreeStoreMixin extends Mixin(
//     [ EventStoreMixin ],
//     (base : AnyConstructor<EventStoreMixin, typeof EventStoreMixin>) => {
//
//     const superProto : InstanceType<typeof base> = base.prototype
//
//
//         class EventTreeStoreMixin extends base {
//             rootNode            : SchedulerBasicProjectMixin
//
//             buildRootNode () : object {
//                 return this.getProject() || {}
//             }
//
//
//             static get defaultConfig () : object {
//                 return {
//                     tree        : true
//                 }
//             }
//         }
//
//         return EventTreeStoreMixin
//     }){}
//

/**
 * A store mixin class, that represent collection of all resources in the [[SchedulerCoreProjectMixin|project]].
 */
class CoreResourceStoreMixin extends Mixin([AbstractResourceStoreMixin, CorePartOfProjectStoreMixin], base => {
  const superProto = base.prototype;
  class CoreResourceStoreMixin extends base {
    static get defaultConfig() {
      return {
        modelClass: CoreResourceMixin
      };
    }
    joinProject() {
      var _this$assignmentStore;
      (_this$assignmentStore = this.assignmentStore) === null || _this$assignmentStore === void 0 ? void 0 : _this$assignmentStore.linkAssignments(this, 'resource');
    }
    afterLoadData() {
      var _this$assignmentStore2;
      (_this$assignmentStore2 = this.assignmentStore) === null || _this$assignmentStore2 === void 0 ? void 0 : _this$assignmentStore2.linkAssignments(this, 'resource');
    }
    clear(removing) {
      var _this$assignmentStore3;
      superProto.clear.call(this, removing);
      (_this$assignmentStore3 = this.assignmentStore) === null || _this$assignmentStore3 === void 0 ? void 0 : _this$assignmentStore3.unlinkAssignments('resource');
    }
  }
  return CoreResourceStoreMixin;
}) {}

class EventsWrapper extends Mixin([], Events) {}
class DelayableWrapper$1 extends Mixin([], Delayable) {}
/**
 * This is an abstract project, which just lists the available stores.
 *
 * The actual project classes are [[SchedulerCoreProjectMixin]], [[SchedulerBasicProjectMixin]],
 * [[SchedulerProProjectMixin]], [[GanttProjectMixin]].
 */
class AbstractProjectMixin extends Mixin([EventsWrapper, DelayableWrapper$1, Model], base => {
  const superProto = base.prototype;
  class AbstractProjectMixin extends base {
    constructor() {
      super(...arguments);
      this.isRestoringData = false;
    }
    get isRepopulatingStores() {
      return false;
    }
    get isInitialCommit() {
      return !this.isInitialCommitPerformed || this.hasLoadedDataToCommit;
    }
    construct(config = {}) {
      // Define default values for these flags here
      // if defined where declared then TS compiles them this way:
      // constructor() {
      //     super(...arguments)
      //     this.isInitialCommitPerformed   = false
      //     this.isLoadingInlineData        = false
      //     this.isWritingData              = false
      //
      // }
      // which messes the flags values for inline data loading (since it's async)
      this.isInitialCommitPerformed = false;
      this.isLoadingInlineData = false;
      this.isWritingData = false;
      this.hasLoadedDataToCommit = false;
      const silenceInitialCommit = 'silenceInitialCommit' in config ? config.silenceInitialCommit : true;
      const adjustDurationToDST = 'adjustDurationToDST' in config ? config.adjustDurationToDST : false;
      // 5 years roughly === 5 * 365 * 24 * 60 * 60 * 1000
      this.maxCalendarRange = 'maxCalendarRange' in config ? config.maxCalendarRange : 157680000000;
      // delete configs otherwise super.construct() call treat them as fields and makes accessors for them
      delete config.maxCalendarRange;
      delete config.silenceInitialCommit;
      delete config.adjustDurationToDST;
      superProto.construct.call(this, config);
      this.silenceInitialCommit = silenceInitialCommit;
      this.adjustDurationToDST = adjustDurationToDST;
    }
    // Template method called when a stores dataset is replaced. Implemented in SchedulerBasicProjectMixin
    repopulateStore(store) {}
    // Template method called when replica should be repopulated. Implemented in SchedulerBasicProjectMixin
    repopulateReplica() {}
    deferUntilRepopulationIfNeeded(deferId, func, args) {
      // no deferring at this level (happens in projects using engine)
      func(...args);
    }
    // Template method called when a store is attached to the project
    attachStore(store) {}
    // Template method called when a store is detached to the project
    detachStore(store) {}
    async commitAsync() {
      throw new Error("Abstract method called");
    }
    // Different implementations for Core and Basic engines
    isEngineReady() {
      throw new Error("Abstract method called");
    }
    getStm() {
      throw new Error("Abstract method called");
    }
  }
  return AbstractProjectMixin;
}) {}

class DelayableWrapper extends Mixin([], Delayable) {}
/**
 * This is a project, implementing _basic scheduling_ as [[SchedulerBasicProjectMixin]] does.
 * Yet this class does not use _chronograph_ based engine.
 */
class SchedulerCoreProjectMixin extends Mixin([AbstractProjectMixin, CorePartOfProjectGenericMixin, DelayableWrapper, Model], base => {
  const superProto = base.prototype;
  class SchedulerCoreProjectMixin extends base {
    static get configurable() {
      return {
        stm: {},
        eventStore: {},
        assignmentStore: {},
        resourceStore: {},
        dependencyStore: {},
        calendarManagerStore: {},
        eventModelClass: SchedulerCoreEvent,
        assignmentModelClass: CoreAssignmentMixin,
        resourceModelClass: CoreResourceMixin,
        dependencyModelClass: CoreDependencyMixin,
        calendarModelClass: CoreCalendarMixin,
        eventStoreClass: CoreEventStoreMixin,
        assignmentStoreClass: CoreAssignmentStoreMixin,
        resourceStoreClass: CoreResourceStoreMixin,
        dependencyStoreClass: CoreDependencyStoreMixin,
        calendarManagerStoreClass: CoreCalendarManagerStoreMixin,
        assignmentsData: null,
        calendarsData: null,
        dependenciesData: null,
        eventsData: null,
        resourcesData: null
      };
    }
    //endregion
    //region Init
    construct(config = {}) {
      const me = this;
      // Cannot be created with declaration, because of how TS is compiled to JS. Ends up after `construct()`
      me.$invalidated = new Set();
      // Define default values for these flags here
      // if defined where declared then TS compiles them this way:
      // constructor() {
      //     super(...arguments)
      //     this.isPerformingCommit   = false
      //     this.silenceInitialCommit = true
      //     this.ongoing              = Promise.resolve()
      // }
      // which messes the flags values for inline data loading (since it's async)
      me.isPerformingCommit = false;
      me.silenceInitialCommit = true;
      me.ongoing = Promise.resolve();
      if (config.eventStore && !config.assignmentStore) {
        var _eventStore$crudManag;
        const eventStore = config.eventStore.masterStore || config.eventStore;
        // If chained from a CrudManager, the assignment store might not be part of a project, and we might
        // need to ingest it from the CrudManager
        // @ts-ignore
        const assignmentStore = eventStore.assignmentStore || ((_eventStore$crudManag = eventStore.crudManager) === null || _eventStore$crudManag === void 0 ? void 0 : _eventStore$crudManag.assignmentStore);
        // In this case we must ingest the assignment store from the eventStore
        if (assignmentStore !== null && assignmentStore !== void 0 && assignmentStore.isAssignmentStore) {
          config.assignmentStore = assignmentStore;
          me.isSharingAssignmentStore = true;
        }
      }
      superProto.construct.call(me, config);
      // not part of the CalendarManagerStore intentionally, not persisted
      me.defaultCalendar = new me.calendarManagerStore.modelClass({
        unspecifiedTimeIsWorking: me.unspecifiedTimeIsWorking
      });
      me.defaultCalendar.project = me;
      const {
        calendarsData,
        eventsData,
        dependenciesData,
        resourcesData,
        assignmentsData
      } = me;
      const hasInlineData = Boolean(calendarsData || eventsData || dependenciesData || resourcesData || assignmentsData);
      if (hasInlineData) {
        me.loadInlineData({
          calendarsData,
          eventsData,
          dependenciesData,
          resourcesData,
          assignmentsData
        });
        delete me.calendarsData;
        delete me.eventsData;
        delete me.dependenciesData;
        delete me.resourcesData;
        delete me.assignmentsData;
      } else {
        // Trigger initial commit
        me.bufferedCommitAsync();
      }
    }
    doDestroy() {
      var _me$eventStore, _me$dependencyStore, _me$assignmentStore, _me$resourceStore, _me$calendarManagerSt, _me$stm;
      const me = this;
      (_me$eventStore = me.eventStore) === null || _me$eventStore === void 0 ? void 0 : _me$eventStore.destroy();
      (_me$dependencyStore = me.dependencyStore) === null || _me$dependencyStore === void 0 ? void 0 : _me$dependencyStore.destroy();
      (_me$assignmentStore = me.assignmentStore) === null || _me$assignmentStore === void 0 ? void 0 : _me$assignmentStore.destroy();
      (_me$resourceStore = me.resourceStore) === null || _me$resourceStore === void 0 ? void 0 : _me$resourceStore.destroy();
      (_me$calendarManagerSt = me.calendarManagerStore) === null || _me$calendarManagerSt === void 0 ? void 0 : _me$calendarManagerSt.destroy();
      me.defaultCalendar.destroy();
      (_me$stm = me.stm) === null || _me$stm === void 0 ? void 0 : _me$stm.destroy();
      superProto.doDestroy.call(this);
    }
    /**
     * This method loads the "raw" data into the project. The loading is basically happening by
     * assigning the individual data entries to the `data` property of the corresponding store.
     *
     * @param data
     */
    async loadInlineData(data) {
      const me = this;
      me.isLoadingInlineData = true;
      if (data.calendarsData) {
        me.calendarManagerStore.data = data.calendarsData;
      }
      if (data.resourcesData) {
        me.resourceStore.data = data.resourcesData;
      }
      if (data.assignmentsData) {
        me.assignmentStore.data = data.assignmentsData;
      }
      if (data.eventsData) {
        me.eventStore.data = data.eventsData;
      }
      if (data.tasksData) {
        me.eventStore.data = data.tasksData;
      }
      if (data.dependenciesData) {
        me.dependencyStore.data = data.dependenciesData;
      }
      await me.commitLoad();
      me.isLoadingInlineData = false;
      return;
    }
    //endregion
    //region Join
    async commitLoad() {
      await this.commitAsync();
      // Might have been destroyed during the async operation above
      if (!this.isDestroyed) this.trigger('load');
    }
    joinStoreRecords(store) {
      const fn = record => {
        record.setProject(this);
        record.joinProject();
      };
      if (store.rootNode) {
        store.rootNode.traverse(fn);
      } else {
        store.forEach(fn);
      }
    }
    unJoinStoreRecords(store) {
      const fn = record => {
        record.leaveProject();
        record.setProject(this);
      };
      if (store.rootNode) {
        store.rootNode.traverse(node => {
          // do not unjoin/leave project for the root node, which is the project itself
          if (node !== store.rootNode) fn(node);
        });
      } else {
        store.forEach(fn);
      }
    }
    //endregion
    //region EventStore
    resolveStoreAndModelClass(name, config) {
      // storeClass from supplied config or our properties
      const storeClass = (config === null || config === void 0 ? void 0 : config.storeClass) || this[`${name}StoreClass`];
      // modelClass from supplied config
      let modelClass = config === null || config === void 0 ? void 0 : config.modelClass;
      if (!modelClass) {
        // or from storeClass.modelClass if customized
        // @ts-ignore
        if (this.getDefaultConfiguration()[`${name}ModelClass`] !== storeClass.getDefaultConfiguration().modelClass) {
          modelClass = storeClass.getDefaultConfiguration().modelClass;
        }
        // and if none of the above, use from our properties
        else {
          modelClass = this[`${name}ModelClass`];
        }
      }
      return {
        storeClass,
        modelClass
      };
    }
    setEventStore(eventStore) {
      this.eventStore = eventStore;
    }
    changeEventStore(eventStore, oldStore) {
      const me = this,
        {
          stm
        } = me;
      me.detachStore(oldStore);
      if (!(eventStore instanceof Store)) {
        const {
          storeClass,
          modelClass
        } = me.resolveStoreAndModelClass('event', eventStore);
        eventStore = new storeClass(ObjectHelper.assign({
          modelClass,
          project: me,
          stm
        }, eventStore));
      } else {
        eventStore.project = me;
        stm.addStore(eventStore);
        me.joinStoreRecords(eventStore);
      }
      if (oldStore && stm.hasStore(oldStore)) {
        stm.removeStore(oldStore);
        me.unJoinStoreRecords(oldStore);
        const {
          assignmentsForRemoval
        } = oldStore;
        // remap the assignment
        assignmentsForRemoval.forEach(assignment => {
          const oldEvent = assignment.event;
          if (oldEvent) {
            const newEvent = eventStore.getById(oldEvent.id);
            if (newEvent) {
              assignment.event = newEvent;
              // keep the assignment
              assignmentsForRemoval.delete(assignment);
            }
          }
        });
        oldStore.afterEventRemoval();
      }
      eventStore.setProject(me);
      return eventStore;
    }
    updateEventStore(eventStore, oldStore) {
      this.attachStore(eventStore);
      this.trigger('eventStoreChange', {
        store: eventStore
      });
    }
    //endregion
    //region AssignmentStore
    setAssignmentStore(assignmentStore) {
      this.assignmentStore = assignmentStore;
    }
    changeAssignmentStore(assignmentStore, oldStore) {
      const me = this,
        {
          stm
        } = me;
      me.detachStore(oldStore);
      if (oldStore && stm.hasStore(oldStore)) {
        stm.removeStore(oldStore);
        me.unJoinStoreRecords(oldStore);
      }
      if (!(assignmentStore instanceof Store)) {
        const {
          storeClass,
          modelClass
        } = me.resolveStoreAndModelClass('assignment', assignmentStore);
        assignmentStore = new storeClass(ObjectHelper.assign({
          modelClass,
          project: me,
          stm
        }, assignmentStore));
      } else {
        assignmentStore.project = me;
        stm.addStore(assignmentStore);
        me.joinStoreRecords(assignmentStore);
      }
      assignmentStore.setProject(me);
      return assignmentStore;
    }
    updateAssignmentStore(assignmentStore, oldStore) {
      this.attachStore(assignmentStore);
      this.trigger('assignmentStoreChange', {
        store: assignmentStore
      });
    }
    //endregion
    //region ResourceStore
    setResourceStore(resourceStore) {
      this.resourceStore = resourceStore;
    }
    changeResourceStore(resourceStore, oldStore) {
      const me = this,
        {
          stm
        } = me;
      me.detachStore(oldStore);
      if (!(resourceStore instanceof Store)) {
        const {
          storeClass,
          modelClass
        } = me.resolveStoreAndModelClass('resource', resourceStore);
        resourceStore = new storeClass(ObjectHelper.assign({
          modelClass,
          project: me,
          stm
        }, resourceStore));
      } else {
        resourceStore.project = me;
        stm.addStore(resourceStore);
        me.joinStoreRecords(resourceStore);
      }
      if (oldStore && stm.hasStore(oldStore)) {
        stm.removeStore(oldStore);
        me.unJoinStoreRecords(oldStore);
        const {
          assignmentsForRemoval
        } = oldStore;
        // remap the assignment
        assignmentsForRemoval.forEach(assignment => {
          const oldResource = assignment.resource;
          if (oldResource) {
            const newResource = resourceStore.getById(oldResource.id);
            if (newResource) {
              assignment.resource = newResource;
              // keep the assignment
              assignmentsForRemoval.delete(assignment);
            }
          }
        });
        oldStore.afterResourceRemoval();
      }
      resourceStore.setProject(me);
      return resourceStore;
    }
    updateResourceStore(resourceStore, oldStore) {
      this.attachStore(resourceStore);
      this.trigger('resourceStoreChange', {
        store: resourceStore
      });
    }
    //endregion
    //region DependencyStore
    setDependencyStore(dependencyStore) {
      this.dependencyStore = dependencyStore;
    }
    changeDependencyStore(dependencyStore, oldStore) {
      const me = this;
      me.detachStore(oldStore);
      if (!(dependencyStore instanceof Store)) {
        const {
          storeClass,
          modelClass
        } = me.resolveStoreAndModelClass('dependency', dependencyStore);
        dependencyStore = new storeClass(ObjectHelper.assign({
          modelClass,
          project: me,
          stm: me.stm
        }, dependencyStore));
      } else {
        dependencyStore.project = me;
        me.stm.addStore(dependencyStore);
        me.joinStoreRecords(dependencyStore);
      }
      return dependencyStore;
    }
    updateDependencyStore(dependencyStore, oldStore) {
      this.attachStore(dependencyStore);
      this.trigger('dependencyStoreChange', {
        store: dependencyStore
      });
    }
    //endregion
    //region CalendarManagerStore
    setCalendarManagerStore(calendarManagerStore) {
      this.calendarManagerStore = calendarManagerStore;
    }
    changeCalendarManagerStore(calendarManagerStore, oldStore) {
      const me = this;
      me.detachStore(oldStore);
      if (!(calendarManagerStore instanceof Store)) {
        var _calendarManagerStore, _calendarManagerStore2;
        // @ts-ignore
        const storeClass = ((_calendarManagerStore = calendarManagerStore) === null || _calendarManagerStore === void 0 ? void 0 : _calendarManagerStore.storeClass) || me.calendarManagerStoreClass;
        // @ts-ignore
        const modelClass = ((_calendarManagerStore2 = calendarManagerStore) === null || _calendarManagerStore2 === void 0 ? void 0 : _calendarManagerStore2.modelClass) || storeClass.getDefaultConfiguration().modelClass || me.calendarModelClass;
        calendarManagerStore = new storeClass(ObjectHelper.assign({
          modelClass,
          project: me,
          stm: me.stm
        }, calendarManagerStore));
      } else {
        me.stm.addStore(calendarManagerStore);
      }
      calendarManagerStore.setProject(me);
      return calendarManagerStore;
    }
    updateCalendarManagerStore(calendarManagerStore, oldStore) {
      this.attachStore(calendarManagerStore);
      this.trigger('calendarManagerStoreChange', {
        store: calendarManagerStore
      });
    }
    //endregion
    //region Calendar
    get calendar() {
      return this.$calendar || this.defaultCalendar;
    }
    set calendar(calendar) {
      this.$calendar = calendar;
    }
    get effectiveCalendar() {
      return this.calendar;
    }
    //endregion
    //region Add records
    async addEvent(event) {
      this.eventStore.add(event);
      return this.commitAsync();
    }
    async addAssignment(assignment) {
      this.assignmentStore.add(assignment);
      return this.commitAsync();
    }
    async addResource(resource) {
      this.resourceStore.add(resource);
      return this.commitAsync();
    }
    async addDependency(dependency) {
      this.dependencyStore.add(dependency);
      return this.commitAsync();
    }
    //endregion
    //region Auto commit
    // Buffer commitAsync using setTimeout. Not using `buffer` on purpose, for performance reasons and to better
    // mimic how graph does it
    bufferedCommitAsync() {
      if (!this.hasPendingAutoCommit) {
        this.setTimeout({
          fn: 'commitAsync',
          delay: 10
        });
      }
    }
    get hasPendingAutoCommit() {
      return this.hasTimeout('commitAsync');
    }
    unScheduleAutoCommit() {
      this.clearTimeout('commitAsync');
    }
    //endregion
    //region Commit
    async commitAsync() {
      if (this.isPerformingCommit) return this.ongoing;
      return this.ongoing = this.doCommitAsync();
    }
    async doCommitAsync() {
      const me = this;
      me.isPerformingCommit = true;
      // Cancel any outstanding commit
      me.unScheduleAutoCommit();
      await delay(0);
      if (!me.isDestroyed) {
        // Calculate all invalidated records, updates their data silently
        for (const record of me.$invalidated) {
          record.calculateInvalidated();
        }
        const {
          isInitialCommit,
          silenceInitialCommit
        } = me;
        // apply changes silently if this is initial commit and "silenceInitialCommit" option is enabled
        const silenceCommit = isInitialCommit && silenceInitialCommit;
        // Notify stores that care about commit (internal)
        me.assignmentStore.onCommitAsync();
        me.dependencyStore.onCommitAsync();
        me.isInitialCommitPerformed = true;
        me.hasLoadedDataToCommit = false;
        me.isPerformingCommit = false;
        const stores = [me.assignmentStore, me.dependencyStore, me.eventStore, me.resourceStore, me.calendarManagerStore];
        stores.forEach(store => {
          var _store$suspendAutoCom;
          return (_store$suspendAutoCom = store.suspendAutoCommit) === null || _store$suspendAutoCom === void 0 ? void 0 : _store$suspendAutoCom.call(store);
        });
        me.isWritingData = true;
        // "Real" project triggers refresh before data is written back to records
        me.trigger('refresh', {
          isInitialCommit,
          isCalculated: true
        });
        // If we are not announcing changes, take a cheaper path
        if (silenceCommit) {
          for (const record of me.$invalidated) {
            record.finalizeInvalidated(true);
          }
        } else {
          // Two loops looks a bit weird, but needed since editing assignment might affect event etc.
          // And we do only want a single update in the end
          // 1. Start batches and perform all calculations
          for (const record of me.$invalidated) {
            record.beginBatch(true);
            record.finalizeInvalidated();
          }
          // 2. End batches, announcing changes (unless initial commit)
          for (const record of me.$invalidated) {
            record.endBatch(false, true);
          }
        }
        me.isWritingData = false;
        me.$invalidated.clear();
        // Mimic real projects events
        me.trigger('dataReady');
        stores.forEach(store => {
          var _store$resumeAutoComm;
          return (_store$resumeAutoComm = store.resumeAutoCommit) === null || _store$resumeAutoComm === void 0 ? void 0 : _store$resumeAutoComm.call(store);
        });
        // Chrono version triggers "dataReady" only if there were no commit rejection
        // (in case of a rejection it triggers "commitRejected" event)
        // but in both cases it triggers "commitFinalized" afterwards
        me.trigger('commitFinalized');
        return true;
      }
    }
    async propagateAsync() {
      return this.commitAsync();
    }
    // Called when a record invalidates itself, queues it for calculation
    invalidate(record) {
      this.$invalidated.add(record);
      this.bufferedCommitAsync();
    }
    // this does not account for possible scheduling conflicts
    async isValidDependency() {
      return true;
    }
    //endregion
    //region STM
    getStm() {
      return this.stm;
    }
    /**
     * State tracking manager instance the project relies on
     */
    set stm(stm) {
      stm = this.$stm = new StateTrackingManager(ObjectHelper.assign({
        disabled: true
      }, stm));
      stm.ion({
        // Propagate on undo/redo
        restoringStop: async () => {
          // Disable STM meanwhile to not pick it up as a new STM transaction
          stm.disable();
          await this.commitAsync();
          if (!this.isDestroyed) {
            stm.enable();
            this.trigger('stateRestoringDone');
          }
        }
      });
    }
    get stm() {
      return this.$stm;
    }
    //endregion
    isEngineReady() {
      return !this.hasPendingAutoCommit && !this.isPerformingCommit && this.isInitialCommitPerformed;
    }
  }
  //region Config
  SchedulerCoreProjectMixin.applyConfigs = true;
  return SchedulerCoreProjectMixin;
}) {}

/**
 * @module Scheduler/crud/mixin/AbstractCrudManagerValidation
 */
/**
 * Mixin proving responses validation API to Crud Manager.
 * @mixin
 */
var AbstractCrudManagerValidation = (Target => class AbstractCrudManagerValidation extends Target {
  static get $name() {
    return 'AbstractCrudManagerValidation';
  }
  static get configurable() {
    return {
      /**
       * This config validates the response structure for requests made by the Crud Manager.
       * When `true`, the Crud Manager checks every parsed response structure for errors
       * and if the response format is invalid, a warning is logged to the browser console.
       *
       * The config is intended to help developers implementing backend integration.
       *
       * @config {Boolean}
       * @default
       * @category CRUD
       */
      validateResponse: true,
      /**
       * When `true` treats parsed responses without `success` property as successful.
       * In this mode a parsed response is treated as invalid if it has explicitly set `success : false`.
       * @config {Boolean}
       * @default
       * @category CRUD
       */
      skipSuccessProperty: true,
      crudLoadValidationWarningPrefix: 'CrudManager load response error(s):',
      crudSyncValidationWarningPrefix: 'CrudManager sync response error(s):',
      supportShortSyncResponseNote: 'Note: Please consider enabling "supportShortSyncResponse" option to allow less detailed sync responses (https://bryntum.com/products/scheduler/docs/api/Scheduler/crud/AbstractCrudManagerMixin#config-supportShortSyncResponse)',
      disableValidationNote: 'Note: To disable this validation please set the "validateResponse" config to false'
    };
  }
  get crudLoadValidationMandatoryStores() {
    return [];
  }
  getStoreLoadResponseWarnings(storeInfo, responded, expectedResponse) {
    const messages = [],
      {
        storeId
      } = storeInfo,
      mandatoryStores = this.crudLoadValidationMandatoryStores,
      result = {
        [storeId]: {}
      };
    // if the store section is responded
    if (responded) {
      if (!responded.rows) {
        messages.push(`- "${storeId}" store section should have a "rows" property with an array of the store records.`);
        result[storeId].rows = ['...'];
      }
    }
    // if the store is mandatory
    else if (mandatoryStores !== null && mandatoryStores !== void 0 && mandatoryStores.includes(storeId)) {
      messages.push(`- No "${storeId}" store section found. It should contain the store data.`);
      result[storeId].rows = ['...'];
    }
    // extend expected response w/ this store part
    if (messages.length) {
      Object.assign(expectedResponse, result);
    }
    return messages;
  }
  getLoadResponseWarnings(response) {
    const messages = [],
      expectedResponse = {};
    if (!this.skipSuccessProperty) {
      expectedResponse.success = true;
    }
    // iterate stores to check properties validity
    this.forEachCrudStore((store, storeId, storeInfo) => {
      messages.push(...this.getStoreLoadResponseWarnings(storeInfo, response === null || response === void 0 ? void 0 : response[storeId], expectedResponse));
    });
    if (messages.length) {
      messages.push('Please adjust your response to look like this:\n' + JSON.stringify(expectedResponse, null, 4).replace(/"\.\.\."/g, '...'));
      messages.push(this.disableValidationNote);
    }
    return messages;
  }
  validateLoadResponse(response) {
    const messages = this.getLoadResponseWarnings(response);
    if (messages.length) {
      console.warn(this.crudLoadValidationWarningPrefix + '\n' + messages.join('\n'));
    }
  }
  getStoreSyncResponseWarnings(storeInfo, requested, responded, expectedResponse) {
    const messages = [],
      missingRows = [],
      missingRemoved = [],
      {
        storeId
      } = storeInfo,
      result = {
        [storeId]: {}
      },
      phantomIdField = storeInfo.phantomIdField || this.phantomIdField,
      {
        modelClass
      } = storeInfo.store,
      {
        idField
      } = modelClass,
      respondedRows = (responded === null || responded === void 0 ? void 0 : responded.rows) || [],
      respondedRemoved = (responded === null || responded === void 0 ? void 0 : responded.removed) || [];
    let showSupportShortSyncResponseNote = false;
    // if added records were passed in the request they should be mentioned in the response
    if (requested !== null && requested !== void 0 && requested.added) {
      missingRows.push(...requested.added.filter(record => {
        return !respondedRows.find(row => row[phantomIdField] == record[phantomIdField]) && !respondedRemoved.find(row => row[phantomIdField] == record[phantomIdField] || row[idField] == record[phantomIdField]);
      }).map(record => ({
        [phantomIdField]: record[phantomIdField],
        [idField]: '...'
      })));
      if (missingRows.length) {
        const missingIds = missingRows.map(row => '#' + row[phantomIdField]).join(', ');
        messages.push(`- "${storeId}" store "rows" section should mention added record(s) ${missingIds} sent in the request. ` + 'It should contain the added records identifiers (both phantom and "real" ones assigned by the backend).');
      }
    }
    // if short responses are enabled
    if (this.supportShortSyncResponse) {
      // if the data is not object, will return error
      if (!missingRows.length && responded) {
        if (typeof responded !== 'object' || Array.isArray(responded)) {
          messages.push(`- "${storeId}" store section should be an Object.`);
          result[storeId]['...'] = '...';
        }
        // for request to edit records, if rows is present, it must be an array
        if (responded.rows && !Array.isArray(responded.rows)) {
          messages.push(`- "${storeId}" store "rows" section should be an array`);
          missingRows.push('...');
        }
        // removed if presented must be an array
        if (responded.removed && !Array.isArray(responded.removed)) {
          messages.push(`- "${storeId}" store "removed" section should be an array:`);
          missingRemoved.push('...');
        }
      }
    }
    // if short responses are disabled
    else {
      // if updated records were passed in the request they should be mentioned in the response
      if (requested !== null && requested !== void 0 && requested.updated) {
        const missingUpdatedRows = requested.updated.filter(record => !respondedRows.find(row => row[idField] == record[idField])).map(record => ({
          [idField]: record[idField]
        }));
        missingRows.push(...missingUpdatedRows);
        if (missingUpdatedRows.length) {
          const missingIds = missingUpdatedRows.map(row => '#' + row[idField]).join(', ');
          messages.push(`- "${storeId}" store "rows" section should mention updated record(s) ${missingIds} sent in the request. ` + `It should contain the updated record identifiers.`);
          showSupportShortSyncResponseNote = true;
        }
      }
      if (missingRows.length) {
        missingRows.push('...');
      }
      // if removed records were passed in the request they should be mentioned in the response
      if (requested !== null && requested !== void 0 && requested.removed) {
        missingRemoved.push(...requested.removed.filter(record => !respondedRows.find(row => row[idField] == record[idField])).map(record => ({
          [idField]: record[idField]
        })));
        if (missingRemoved.length) {
          const missingIds = missingRemoved.map(row => '#' + row[idField]).join(', ');
          messages.push(`- "${storeId}" store "removed" section should mention removed record(s) ${missingIds} sent in the request. ` + `It should contain the removed record identifiers.`);
          result[storeId].removed = missingRemoved;
          missingRemoved.push('...');
          showSupportShortSyncResponseNote = true;
        }
      }
    }
    if (missingRows.length) {
      result[storeId].rows = missingRows;
    }
    // get rid of store section if no rows/removed there
    if (!messages.length) {
      delete result[storeId];
    }
    // extend expected response w/ this store part
    Object.assign(expectedResponse, result);
    return {
      messages,
      showSupportShortSyncResponseNote
    };
  }
  getSyncResponseWarnings(response, requestDesc) {
    const messages = [],
      expectedResponse = {},
      request = requestDesc.pack;
    if (!this.skipSuccessProperty) {
      expectedResponse.success = true;
    }
    let showSupportShortSyncResponseNote = false;
    // iterate stores to check properties validity
    this.forEachCrudStore((store, storeId, storeInfo) => {
      const warnings = this.getStoreSyncResponseWarnings(storeInfo, request === null || request === void 0 ? void 0 : request[storeId], response[storeId], expectedResponse);
      showSupportShortSyncResponseNote = showSupportShortSyncResponseNote || warnings.showSupportShortSyncResponseNote;
      messages.push(...warnings.messages);
    });
    if (messages.length) {
      messages.push('Please adjust your response to look like this:\n' + JSON.stringify(expectedResponse, null, 4).replace(/"\.\.\.":\s*"\.\.\."/g, ',,,').replace(/"\.\.\."/g, '...'));
      if (showSupportShortSyncResponseNote) {
        messages.push(this.supportShortSyncResponseNote);
      }
      messages.push(this.disableValidationNote);
    }
    return messages;
  }
  validateSyncResponse(response, request) {
    const messages = this.getSyncResponseWarnings(response, request);
    if (messages.length) {
      console.warn(this.crudSyncValidationWarningPrefix + '\n' + messages.join('\n'));
    }
  }
});

/**
 * @module Scheduler/crud/AbstractCrudManagerMixin
 */
class AbstractCrudManagerError extends Error {}
class CrudManagerRequestError extends AbstractCrudManagerError {
  constructor(cfg = {}) {
    var _cfg$request, _this$request;
    super(cfg.message || cfg.request && StringHelper.capitalize((_cfg$request = cfg.request) === null || _cfg$request === void 0 ? void 0 : _cfg$request.type) + ' failed' || 'Crud Manager request failed');
    Object.assign(this, cfg);
    this.action = (_this$request = this.request) === null || _this$request === void 0 ? void 0 : _this$request.type;
  }
}
const storeSortFn = function (lhs, rhs, sortProperty) {
    if (lhs.store) {
      lhs = lhs.store;
    }
    if (rhs.store) {
      rhs = rhs.store;
    }
    lhs = lhs[sortProperty] || 0;
    rhs = rhs[sortProperty] || 0;
    return lhs < rhs ? -1 : lhs > rhs ? 1 : 0;
  },
  // Sorter function to keep stores in loadPriority order
  storeLoadSortFn = function (lhs, rhs) {
    return storeSortFn(lhs, rhs, 'loadPriority');
  },
  // Sorter function to keep stores in syncPriority order
  storeSyncSortFn = function (lhs, rhs) {
    return storeSortFn(lhs, rhs, 'syncPriority');
  };
/**
 * An abstract mixin that supplies most of the CrudManager functionality.
 * It implements basic mechanisms of collecting stores to organize batch communication with a server.
 * It does not contain methods related to _data transfer_ nor _encoding_.
 * These methods are to be provided in sub-classes.
 * Out of the box there are mixins implementing {@link Scheduler/crud/transport/AjaxTransport support of AJAX for data transferring}
 * and {@link Scheduler/crud/encoder/JsonEncoder JSON for data encoding system}.
 * For example this is how we make a model that will implement CrudManager protocol and use AJAX/JSON to pass the data
 * to the server:
 *
 * ```javascript
 * class SystemSettings extends JsonEncode(AjaxTransport(AbstractCrudManagerMixin(Model))) {
 *     ...
 * }
 * ```
 *
 * ## Data transfer and encoding methods
 *
 * These are methods that must be provided by subclasses of this class:
 *
 * - {@link #function-sendRequest}
 * - {@link #function-cancelRequest}
 * - {@link #function-encode}
 * - {@link #function-decode}
 *
 * @mixin
 * @mixes Core/mixin/Delayable
 * @mixes Core/mixin/Events
 * @mixes Scheduler/crud/mixin/AbstractCrudManagerValidation
 * @abstract
 */
var AbstractCrudManagerMixin = (Target => {
  // Trigger $meta calculation to get up-to-date is "isXXX" flags
  // (kinky construction to avoid production minification faced in Angular https://github.com/bryntum/support/issues/2889)
  Target.$$meta = Target.$meta;
  const mixins = [];
  // These two mixins are mixed in the Scheduling Engine code ..but in its own way
  // so that Base.mixin() cannot understand that they are already there and applies them 2nd time
  if (!Target.isEvents) {
    mixins.push(Events);
  }
  if (!Target.isDelayable) {
    mixins.push(Delayable);
  }
  mixins.push(AbstractCrudManagerValidation);
  return class AbstractCrudManagerMixin extends (Target || Base).mixin(...mixins) {
    /**
     * Fires before server response gets applied to the stores. Return `false` to prevent data applying.
     * This event can be used for server data preprocessing. To achieve it user can modify the `response` object.
     * @event beforeResponseApply
     * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
     * @param {'sync'|'load'} requestType The request type (`sync` or `load`).
     * @param {Object} response The decoded server response object.
     */
    /**
     * Fires before loaded data get applied to the stores. Return `false` to prevent data applying.
     * This event can be used for server data preprocessing. To achieve it user can modify the `response` object.
     * @event beforeLoadApply
     * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
     * @param {Object} response The decoded server response object.
     * @param {Object} options Options provided to the {@link #function-load} method.
     */
    /**
     * Fires before sync response data get applied to the stores. Return `false` to prevent data applying.
     * This event can be used for server data preprocessing. To achieve it user can modify the `response` object.
     * @event beforeSyncApply
     * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
     * @param {Object} response The decoded server response object.
     */
    static get $name() {
      return 'AbstractCrudManagerMixin';
    }
    //region Default config
    static get defaultConfig() {
      return {
        /**
         * The server revision stamp.
         * The _revision stamp_ is a number which should be incremented after each server-side change.
         * This property reflects the current version of the data retrieved from the server and gets updated
         * after each {@link #function-load} and {@link #function-sync} call.
         * @property {Number}
         * @readonly
         * @category CRUD
         */
        crudRevision: null,
        /**
         * A list of registered stores whose server communication will be collected into a single batch.
         * Each store is represented by a _store descriptor_.
         * @member {CrudManagerStoreDescriptor[]} crudStores
         * @category CRUD
         */
        /**
         * Sets the list of stores controlled by the CRUD manager.
         *
         * When adding a store to the CrudManager, make sure the server response format is correct for `load`
         * and `sync` requests. Learn more in the
         * [Working with data](#Scheduler/guides/data/crud_manager.md#loading-data) guide.
         *
         * Store can be provided by itself, its storeId or as a _store descriptor_.
         * @config {Core.data.Store[]|String[]|CrudManagerStoreDescriptor[]}
         * @category CRUD
         */
        crudStores: [],
        /**
         * Name of a store property to retrieve store identifiers from. Make sure you have an instance of a
         * store to use it by id. Store identifier is used as a container name holding corresponding store data
         * while transferring them to/from the server. By default, `storeId` property is used. And in case a
         * container identifier has to differ this config can be used:
         *
         * ```javascript
         * class CatStore extends Store {
         *     static configurable = {
         *         // store id is "meow" but for sending/receiving store data
         *         // we want to have "cats" container in JSON, so we create a new property "storeIdForCrud"
         *         id             : 'meow',
         *         storeIdForCrud : 'cats'
         *     }
         * });
         *
         * // create an instance to use a store by id
         * new CatStore();
         *
         * class MyCrudManager extends CrudManager {
         *     ...
         *     crudStores           : ['meow'],
         *     // crud manager will get store identifier from "storeIdForCrud" property
         *     storeIdProperty  : 'storeIdForCrud'
         * });
         * ```
         * The `storeIdProperty` property can also be specified directly on a store:
         *
         * ```javascript
         * class CatStore extends Store {
         *     static configurable = {
         *         // storeId is "meow" but for sending/receiving store data
         *         // we want to have "cats" container in JSON
         *         id              : 'meow',
         *         // so we create a new property "storeIdForCrud"..
         *         storeIdForCrud  : 'cats',
         *         // and point CrudManager to use it as the store identifier source
         *         storeIdProperty  : 'storeIdForCrud'
         *     }
         * });
         *
         * class DogStore extends Store {
         *     static configurable = {
         *         // storeId is "dogs" and it will be used as a container name for the store data
         *         storeId : 'dogs',
         *         // id is set to get a store by identifier
         *         id      : 'dogs'
         *     }
         * });
         *
         * // create an instance to use a store by id
         * new CatStore();
         * new DogStore();
         *
         * class MyCrudManager extends CrudManager {
         *     ...
         *     crudStores : ['meow', 'dogs']
         * });
         * ```
         * @config {String}
         * @category CRUD
         */
        storeIdProperty: 'storeId',
        crudFilterParam: 'filter',
        /**
         * Sends request to the server.
         * @function sendRequest
         * @param {Object} request The request to send. An object having following properties:
         * @param {'load'|'sync'} request.type Request type, can be either `load` or `sync`
         * @param {String} request.data {@link #function-encode Encoded} request.
         * @param {Function} request.success Callback to be started on successful request transferring
         * @param {Function} request.failure Callback to be started on request transfer failure
         * @param {Object} request.thisObj `this` reference for the above `success` and `failure` callbacks
         * @returns {Promise} The request promise.
         * @abstract
         */
        /**
         * Cancels request to the server.
         * @function cancelRequest
         * @param {Promise} promise The request promise to cancel (a value returned by corresponding
         * {@link #function-sendRequest} call).
         * @param {Function} reject Reject handle of the corresponding promise
         * @abstract
         */
        /**
         * Encodes request to the server.
         * @function encode
         * @param {Object} request The request to encode.
         * @returns {String} The encoded request.
         * @abstract
         */
        /**
         * Decodes response from the server.
         * @function decode
         * @param {String} response The response to decode.
         * @returns {Object} The decoded response.
         * @abstract
         */
        transport: {},
        /**
         * When `true` forces the CRUD manager to process responses depending on their `type` attribute.
         * So `load` request may be responded with `sync` response for example.
         * Can be used for smart server logic allowing the server to decide when it's better to respond with a
         * complete data set (`load` response) or it's enough to return just a delta (`sync` response).
         * @config {Boolean}
         * @default
         * @category CRUD
         */
        trackResponseType: false,
        /**
         * When `true` the Crud Manager does not require all updated and removed records to be mentioned in the
         * *sync* response. In this case response should include only server side changes.
         *
         * **Please note that added records should still be mentioned in response to provide real identifier
         * instead of the phantom one.**
         * @config {Boolean}
         * @default
         * @category CRUD
         */
        supportShortSyncResponse: true,
        /**
         * Field name to be used to transfer a phantom record identifier.
         * @config {String}
         * @default
         * @category CRUD
         */
        phantomIdField: '$PhantomId',
        /**
         * Field name to be used to transfer a phantom parent record identifier.
         * @config {String}
         * @default
         * @category CRUD
         */
        phantomParentIdField: '$PhantomParentId',
        /**
         * Specify `true` to automatically call {@link #function-load} method on the next frame after creation.
         *
         * Called on the next frame to allow a Scheduler (or similar) linked to a standalone CrudManager to
         * register its stores before loading starts.
         *
         * @config {Boolean}
         * @default
         * @category CRUD
         */
        autoLoad: false,
        /**
         * The timeout in milliseconds to wait before persisting changes to the server.
         * Used when {@link #config-autoSync} is set to `true`.
         * @config {Number}
         * @default
         * @category CRUD
         */
        autoSyncTimeout: 100,
        /**
         * `true` to automatically persist store changes after edits are made in any of the stores monitored.
         * Please note that sync request will not be invoked immediately but only after
         * {@link #config-autoSyncTimeout} interval.
         * @config {Boolean}
         * @default
         * @category CRUD
         */
        autoSync: false,
        /**
         * `True` to reset identifiers (defined by `idField` config) of phantom records before submitting them
         * to the server.
         * @config {Boolean}
         * @default
         * @category CRUD
         */
        resetIdsBeforeSync: true,
        /**
         * @member {CrudManagerStoreDescriptor[]} syncApplySequence
         * An array of stores presenting an alternative sync responses apply order.
         * Each store is represented by a _store descriptor_.
         * @category CRUD
         */
        /**
         * An array of store identifiers sets an alternative sync responses apply order.
         * By default, the order in which sync responses are applied to the stores is the same as they
         * registered in. But in case of some tricky dependencies between stores this order can be changed:
         *
         *```javascript
         * class MyCrudManager extends CrudManager {
         *     // register stores (will be loaded in this order: 'store1' then 'store2' and finally 'store3')
         *     crudStores : ['store1', 'store2', 'store3'],
         *     // but we apply changes from server to them in an opposite order
         *     syncApplySequence : ['store3', 'store2', 'store1']
         * });
         *```
         * @config {String[]}
         * @category CRUD
         */
        syncApplySequence: [],
        orderedCrudStores: [],
        /**
         * `true` to write all fields from the record to the server.
         * If set to `false` it will only send the fields that were modified.
         * Note that any fields that have {@link Core/data/field/DataField#config-persist} set to `false` will
         * still be ignored and fields having {@link Core/data/field/DataField#config-alwaysWrite} set to `true`
         * will always be included.
         * @config {Boolean}
         * @default
         * @category CRUD
         */
        writeAllFields: false,
        crudIgnoreUpdates: 0,
        autoSyncSuspendCounter: 0,
        // Flag that shows if crud manager performed successful load request
        crudLoaded: false,
        applyingLoadResponse: false,
        applyingSyncResponse: false,
        callOnFunctions: true
      };
    }
    static configurable = {
      /**
       * Convenience shortcut to set only the url to load from, when you do not need to supply any other config
       * options in the `load` section of the `transport` config.
       *
       * Using `loadUrl`:
       * ```javascript
       * {
       *     loadUrl : 'read.php
       * }
       * ```
       *
       * Equals the following `transport` config:
       * ```javascript
       * {
       *     transport : {
       *         load : {
       *             url : 'read.php'
       *         }
       *     }
       * }
       * ```
       *
       * When read at runtime, it will return the value from `transport.load.url`.
       *
       * @prp {String}
       */
      loadUrl: null,
      /**
       * Convenience shortcut to set only the url to sync to, when you do not need to supply any other config
       * options in the `sync` section of the `transport` config.
       *
       * Using `loadUrl`:
       * ```javascript
       * {
       *     syncUrl : 'sync.php
       * }
       * ```
       *
       * Equals the following `transport` config:
       * ```javascript
       * {
       *     transport : {
       *         load : {
       *             url : 'sync.php'
       *         }
       *     }
       * }
       * ```
       *
       * When read at runtime, it will return the value from `transport.sync.url`.
       *
       * @prp {String}
       */
      syncUrl: null,
      /**
       * Specify as `true` to force sync requests to be sent when calling `sync()`, even if there are no local
       * changes. Useful in a polling scenario, to keep client up to date with the backend.
       * @prp {Boolean}
       */
      forceSync: null
    };
    static delayable = {
      // Postponed to next frame, to allow Scheduler created after CrudManager to inject its stores
      // (timeRanges, resourceTimeRanges)
      doAutoLoad: 'raf'
    };
    get isCrudManager() {
      return true;
    }
    //endregion
    //region Init
    construct(config = {}) {
      this._requestId = 0;
      this.activeRequests = {};
      this.crudStoresIndex = {};
      super.construct(config);
    }
    afterConstruct() {
      super.afterConstruct();
      if (this.autoLoad) {
        this._autoLoadPromise = this.doAutoLoad();
      }
    }
    //endregion
    //region Configs
    get loadUrl() {
      var _this$transport, _this$transport$load;
      return (_this$transport = this.transport) === null || _this$transport === void 0 ? void 0 : (_this$transport$load = _this$transport.load) === null || _this$transport$load === void 0 ? void 0 : _this$transport$load.url;
    }
    updateLoadUrl(url) {
      ObjectHelper.setPath(this, 'transport.load.url', url);
    }
    get syncUrl() {
      var _this$transport2, _this$transport2$sync;
      return (_this$transport2 = this.transport) === null || _this$transport2 === void 0 ? void 0 : (_this$transport2$sync = _this$transport2.sync) === null || _this$transport2$sync === void 0 ? void 0 : _this$transport2$sync.url;
    }
    updateSyncUrl(url) {
      ObjectHelper.setPath(this, 'transport.sync.url', url);
    }
    //endregion
    //region Store descriptors & index
    /**
     * Returns a registered store descriptor.
     * @param {String|Core.data.Store} storeId The store identifier or registered store instance.
     * @returns {CrudManagerStoreDescriptor} The descriptor of the store.
     * @category CRUD
     */
    getStoreDescriptor(storeId) {
      if (!storeId) return null;
      if (storeId instanceof Store) return this.crudStores.find(storeDesc => storeDesc.store === storeId);
      if (typeof storeId === 'object') return this.crudStoresIndex[storeId.storeId];
      return this.crudStoresIndex[storeId] || this.getStoreDescriptor(Store.getStore(storeId));
    }
    fillStoreDescriptor(descriptor) {
      const {
          store
        } = descriptor,
        {
          storeIdProperty = this.storeIdProperty,
          modelClass
        } = store;
      if (!descriptor.storeId) {
        descriptor.storeId = store[storeIdProperty] || store.id;
      }
      if (!descriptor.idField) {
        descriptor.idField = modelClass.idField;
      }
      if (!descriptor.phantomIdField) {
        descriptor.phantomIdField = modelClass.phantomIdField;
      }
      if (!descriptor.phantomParentIdField) {
        descriptor.phantomParentIdField = modelClass.phantomParentIdField;
      }
      if (!('writeAllFields' in descriptor)) {
        descriptor.writeAllFields = store.writeAllFields;
      }
      return descriptor;
    }
    updateCrudStoreIndex() {
      const crudStoresIndex = this.crudStoresIndex = {};
      this.crudStores.forEach(store => store.storeId && (crudStoresIndex[store.storeId] = store));
    }
    //endregion
    //region Store collection (add, remove, get & iterate)
    /**
     * Returns a registered store.
     * @param {String} storeId Store identifier.
     * @returns {Core.data.Store} Found store instance.
     * @category CRUD
     */
    getCrudStore(storeId) {
      const storeDescriptor = this.getStoreDescriptor(storeId);
      return storeDescriptor === null || storeDescriptor === void 0 ? void 0 : storeDescriptor.store;
    }
    forEachCrudStore(fn, thisObj = this) {
      if (!fn) {
        throw new Error('Iterator function must be provided');
      }
      this.crudStores.every(store => fn.call(thisObj, store.store, store.storeId, store) !== false);
    }
    set crudStores(stores) {
      this._crudStores = [];
      this.addCrudStore(stores);
      // Ensure preconfigured stores stay stable at the start of the array when
      // addPrioritizedStore attempts to insert in order. Only featured gantt/scheduler stores
      // must participate in the ordering. If they were configured in, they must not move.
      for (const store of this._crudStores) {
        store.loadPriority = store.syncPriority = 0;
      }
    }
    get crudStores() {
      return this._crudStores;
    }
    get orderedCrudStores() {
      return this._orderedCrudStores;
    }
    set orderedCrudStores(stores) {
      return this._orderedCrudStores = stores;
    }
    set syncApplySequence(stores) {
      this._syncApplySequence = [];
      this.addStoreToApplySequence(stores);
    }
    get syncApplySequence() {
      return this._syncApplySequence;
    }
    internalAddCrudStore(store) {
      const me = this;
      let storeInfo;
      // if store instance provided
      if (store instanceof Store) {
        storeInfo = {
          store
        };
      } else if (typeof store === 'object') {
        if (!store.store) {
          // not a store descriptor, assume it is a store config
          store = {
            storeId: store.id,
            store: new Store(store)
          };
        }
        storeInfo = store;
      }
      // if it's a store identifier
      else {
        storeInfo = {
          store: Store.getStore(store)
        };
      }
      me.fillStoreDescriptor(storeInfo);
      // store instance
      store = storeInfo.store;
      // if the store has "setCrudManager" hook - use it
      if (store.setCrudManager) {
        store.setCrudManager(me);
      }
      // otherwise decorate the store w/ "crudManager" property
      else {
        store.crudManager = me;
      }
      // Stores have a defaultConfig for pageSize. CrudManager does not support that.
      store.pageSize = null;
      // Prevent AjaxStores from performing their own CRUD operations if CrudManager is configured with an URL
      if (me.loadUrl || me.syncUrl) {
        store.autoCommit = false;
        store.autoLoad = false;
        if (store.createUrl || store.updateUrl || store.deleteUrl || store.readUrl) {
          console.warn('You have configured an URL on a Store that is handled by a CrudManager that is also configured with an URL. The Store URL\'s should be removed.');
        }
      }
      // listen to store changes
      me.bindCrudStoreListeners(store);
      return storeInfo;
    }
    /**
     * Adds a store to the collection.
     *
     *```javascript
     * // append stores to the end of collection
     * crudManager.addCrudStore([
     *     store1,
     *     // storeId
     *     'bar',
     *     // store descriptor
     *     {
     *         storeId : 'foo',
     *         store   : store3
     *     },
     *     {
     *         storeId         : 'bar',
     *         store           : store4,
     *         // to write all fields of modified records
     *         writeAllFields  : true
     *     }
     * ]);
     *```
     *
     * **Note:** Order in which stores are kept in the collection is very essential sometimes.
     * Exactly in this order the loaded data will be put into each store.
     *
     * When adding a store to the CrudManager, make sure the server response format is correct for `load` and `sync`
     * requests. Learn more in the [Working with data](#Scheduler/guides/data/crud_manager.md#loading-data) guide.
     *
     * @param {Core.data.Store|String|CrudManagerStoreDescriptor|Core.data.Store[]|String[]|CrudManagerStoreDescriptor[]} store
     * A store or list of stores. Each store might be specified by its instance, `storeId` or _descriptor_.
     * @param {Number} [position] The relative position of the store. If `fromStore` is specified the position
     * will be taken relative to it. If not specified then store(s) will be appended to the end of collection.
     * Otherwise, it will be just a position in stores collection.
     *
     * ```javascript
     * // insert stores store4, store5 to the start of collection
     * crudManager.addCrudStore([ store4, store5 ], 0);
     * ```
     *
     * @param {String|Core.data.Store|CrudManagerStoreDescriptor} [fromStore] The store relative to which position
     * should be calculated. Can be defined as a store identifier, instance or descriptor (the result of
     * {@link #function-getStoreDescriptor} call).
     *
     * ```javascript
     * // insert store6 just before a store having storeId equal to 'foo'
     * crudManager.addCrudStore(store6, 0, 'foo');
     *
     * // insert store7 just after store3 store
     * crudManager.addCrudStore(store7, 1, store3);
     * ```
     * @category CRUD
     */
    addCrudStore(store, position, fromStore) {
      var _store;
      store = ArrayHelper.asArray(store);
      if (!((_store = store) !== null && _store !== void 0 && _store.length)) {
        return;
      }
      const me = this,
        stores = store.map(me.internalAddCrudStore, me);
      // if no position specified then append stores to the end
      if (typeof position === 'undefined') {
        me.crudStores.push(...stores);
      }
      // if position specified
      else {
        // if specified the store relative to which we should insert new one(-s)
        if (fromStore) {
          if (fromStore instanceof Store || typeof fromStore !== 'object') fromStore = me.getStoreDescriptor(fromStore);
          // get its position
          position += me.crudStores.indexOf(fromStore);
        }
        // insert new store(-s)
        me.crudStores.splice(position, 0, ...stores);
      }
      me.orderedCrudStores.push(...stores);
      me.updateCrudStoreIndex();
    }
    // Adds configured scheduler stores to the store collection ensuring correct order
    // unless they're already registered.
    addPrioritizedStore(store) {
      const me = this;
      if (!me.hasCrudStore(store)) {
        me.addCrudStore(store, ArrayHelper.findInsertionIndex(store, me.crudStores, storeLoadSortFn));
      }
      if (!me.hasApplySequenceStore(store)) {
        me.addStoreToApplySequence(store, ArrayHelper.findInsertionIndex(store, me.syncApplySequence, storeSyncSortFn));
      }
    }
    hasCrudStore(store) {
      var _this$crudStores;
      return (_this$crudStores = this.crudStores) === null || _this$crudStores === void 0 ? void 0 : _this$crudStores.some(s => s === store || s.store === store || s.storeId === store);
    }
    /**
     * Removes a store from collection. If the store was registered in alternative sync sequence list
     * it will be removed from there as well.
     *
     * ```javascript
     * // remove store having storeId equal to "foo"
     * crudManager.removeCrudStore("foo");
     *
     * // remove store3
     * crudManager.removeCrudStore(store3);
     * ```
     *
     * @param {CrudManagerStoreDescriptor|String|Core.data.Store} store The store to remove. Either the store
     * descriptor, store identifier or store itself.
     * @category CRUD
     */
    removeCrudStore(store) {
      const me = this,
        stores = me.crudStores,
        foundStore = stores.find(s => s === store || s.store === store || s.storeId === store);
      if (foundStore) {
        // unbind store listeners
        me.unbindCrudStoreListeners(foundStore.store);
        delete me.crudStoresIndex[foundStore.storeId];
        ArrayHelper.remove(stores, foundStore);
        if (me.syncApplySequence) {
          me.removeStoreFromApplySequence(store);
        }
      } else {
        throw new Error('Store not found in stores collection');
      }
    }
    //endregion
    //region Store listeners
    bindCrudStoreListeners(store) {
      store.ion({
        name: store.id,
        // When a tentatively added record gets confirmed as permanent, this signals a change
        addConfirmed: 'onCrudStoreChange',
        change: 'onCrudStoreChange',
        destroy: 'onCrudStoreDestroy',
        thisObj: this
      });
    }
    unbindCrudStoreListeners(store) {
      this.detachListeners(store.id);
    }
    //endregion
    //region Apply sequence
    /**
     * Adds a store to the alternative sync responses apply sequence.
     * By default, the order in which sync responses are applied to the stores is the same as they registered in.
     * But this order can be changes either on construction step using {@link #config-syncApplySequence} option
     * or by calling this method.
     *
     * **Please note**, that if the sequence was not initialized before this method call then
     * you will have to do it yourself like this for example:
     *
     * ```javascript
     * // alternative sequence was not set for this crud manager
     * // so let's fill it with existing stores keeping the same order
     * crudManager.addStoreToApplySequence(crudManager.crudStores);
     *
     * // and now we can add our new store
     *
     * // we will load its data last
     * crudManager.addCrudStore(someNewStore);
     * // but changes to it will be applied first
     * crudManager.addStoreToApplySequence(someNewStore, 0);
     * ```
     * add registered stores to the sequence along with the store(s) you want to add
     *
     * @param {Core.data.Store|CrudManagerStoreDescriptor|Core.data.Store[]|CrudManagerStoreDescriptor[]} store The
     * store to add or its _descriptor_ (or array of stores or descriptors).
     * @param {Number} [position] The relative position of the store. If `fromStore` is specified the position
     * will be taken relative to it. If not specified then store(s) will be appended to the end of collection.
     * Otherwise, it will be just a position in stores collection.
     *
     * ```javascript
     * // insert stores store4, store5 to the start of sequence
     * crudManager.addStoreToApplySequence([ store4, store5 ], 0);
     * ```
     * @param {String|Core.data.Store|CrudManagerStoreDescriptor} [fromStore] The store relative to which position
     * should be calculated. Can be defined as a store identifier, instance or its descriptor (the result of
     * {@link #function-getStoreDescriptor} call).
     *
     * ```javascript
     * // insert store6 just before a store having storeId equal to 'foo'
     * crudManager.addStoreToApplySequence(store6, 0, 'foo');
     *
     * // insert store7 just after store3 store
     * crudManager.addStoreToApplySequence(store7, 1, store3);
     * ```
     * @category CRUD
     */
    addStoreToApplySequence(store, position, fromStore) {
      if (!store) {
        return;
      }
      store = ArrayHelper.asArray(store);
      const me = this,
        // loop over list of stores to add
        data = store.reduce((collection, store) => {
          const s = me.getStoreDescriptor(store);
          s && collection.push(s);
          return collection;
        }, []);
      // if no position specified then append stores to the end
      if (typeof position === 'undefined') {
        me.syncApplySequence.push(...data);
        // if position specified
      } else {
        let pos = position;
        // if specified the store relative to which we should insert new one(-s)
        if (fromStore) {
          if (fromStore instanceof Store || typeof fromStore !== 'object') fromStore = me.getStoreDescriptor(fromStore);
          // get its position
          pos += me.syncApplySequence.indexOf(fromStore);
        }
        // insert new store(-s)
        //me.syncApplySequence.splice.apply(me.syncApplySequence, [].concat([pos, 0], data));
        me.syncApplySequence.splice(pos, 0, ...data);
      }
      const sequenceKeys = me.syncApplySequence.map(({
        storeId
      }) => storeId);
      me.orderedCrudStores = [...me.syncApplySequence];
      me.crudStores.forEach(storeDesc => {
        if (!sequenceKeys.includes(storeDesc.storeId)) {
          me.orderedCrudStores.push(storeDesc);
        }
      });
    }
    /**
     * Removes a store from the alternative sync sequence.
     *
     * ```javascript
     * // remove store having storeId equal to "foo"
     * crudManager.removeStoreFromApplySequence("foo");
     * ```
     *
     * @param {CrudManagerStoreDescriptor|String|Core.data.Store} store The store to remove. Either the store
     * descriptor, store identifier or store itself.
     * @category CRUD
     */
    removeStoreFromApplySequence(store) {
      const index = this.syncApplySequence.findIndex(s => s === store || s.store === store || s.storeId === store);
      if (index > -1) {
        this.syncApplySequence.splice(index, 1);
        // ordered crud stores list starts with syncApplySequence, we can use same index
        this.orderedCrudStores.splice(index, 1);
      }
    }
    hasApplySequenceStore(store) {
      return this.syncApplySequence.some(s => s === store || s.store === store || s.storeId === store);
    }
    //endregion
    //region Events
    // Remove stores that are destroyed, to not try and apply response changes etc. to them
    onCrudStoreDestroy({
      source: store
    }) {
      this.removeCrudStore(store);
    }
    onCrudStoreChange(event) {
      const me = this;
      if (me.crudIgnoreUpdates) {
        return;
      }
      /**
       * Fires when data in any of the registered data stores is changed.
       * ```javascript
       *     crudManager.on('hasChanges', function (crud) {
       *         // enable persist changes button when some store gets changed
       *         saveButton.enable();
       *     });
       * ```
       * @event hasChanges
       * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
       */
      if (me.crudStoreHasChanges(event === null || event === void 0 ? void 0 : event.source)) {
        me.trigger('hasChanges');
        if (me.autoSync) {
          me.scheduleAutoSync();
        }
      } else {
        me.trigger('noChanges');
      }
    }
    /**
     * Suspends automatic sync upon store changes. Can be called multiple times (it uses an internal counter).
     * @category CRUD
     */
    suspendAutoSync() {
      this.autoSyncSuspendCounter++;
    }
    /**
     * Resumes automatic sync upon store changes. Will schedule a sync if the internal counter is 0.
     * @param {Boolean} [doSync=true] Pass `true` to schedule a sync after resuming (if there are pending
     * changes) and `false` to not persist the changes.
     * @category CRUD
     */
    resumeAutoSync(doSync = true) {
      const me = this;
      me.autoSyncSuspendCounter--;
      if (me.autoSyncSuspendCounter <= 0) {
        me.autoSyncSuspendCounter = 0;
        // if configured to trigger persisting and there are changes
        if (doSync && me.autoSync && me.crudStoreHasChanges()) {
          me.scheduleAutoSync();
        }
      }
    }
    get isAutoSyncSuspended() {
      return this.autoSyncSuspendCounter > 0;
    }
    scheduleAutoSync() {
      const me = this;
      // add deferred call if it's not scheduled yet
      if (!me.hasTimeout('autoSync') && !me.isAutoSyncSuspended) {
        me.setTimeout({
          name: 'autoSync',
          fn: () => {
            me.sync().catch(error => {});
          },
          delay: me.autoSyncTimeout
        });
      }
    }
    async triggerFailedRequestEvents(request, response, responseText, fetchOptions) {
      const {
        options,
        type: requestType
      } = request;
      /**
       * Fires when a request fails.
       * @event requestFail
       * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager instance.
       * @param {'sync'|'load'} requestType The request type (`sync` or `load`).
       * @param {Object} response The decoded server response object.
       * @param {String} responseText The raw server response text
       * @param {Object} responseOptions The response options.
       */
      this.trigger('requestFail', {
        requestType,
        response,
        responseText,
        responseOptions: fetchOptions
      });
      /**
       * Fires when a {@link #function-load load request} fails.
       * @event loadFail
       * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager instance.
       * @param {Object} response The decoded server response object.
       * @param {String} responseText The raw server response text
       * @param {Object} responseOptions The response options.
       * @params {Object} options Options provided to the {@link #function-load} method.
       */
      /**
       * Fires when a {@link #function-sync sync request} fails.
       * @event syncFail
       * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager instance.
       * @param {Object} response The decoded server response object.
       * @param {String} responseText The raw server response text
       * @param {Object} responseOptions The response options.
       */
      this.trigger(requestType + 'Fail', {
        response,
        responseOptions: fetchOptions,
        responseText,
        options
      });
    }
    async internalOnResponse(request, responseText, fetchOptions) {
      const me = this,
        response = responseText ? me.decode(responseText) : null,
        {
          options,
          type: requestType
        } = request;
      if (responseText && !response) {
        console.error('Failed to parse response: ' + responseText);
      }
      if (!response || (me.skipSuccessProperty ? response.success === false : !response.success)) {
        me.triggerFailedRequestEvents(request, response, responseText, fetchOptions);
      } else if (me.trigger('beforeResponseApply', {
        requestType,
        response
      }) !== false && me.trigger(`before${StringHelper.capitalize(requestType)}Apply`, {
        response,
        options
      }) !== false) {
        me.crudRevision = response.revision;
        await me.applyResponse(request, response, options);
        // Might have been destroyed while applying response
        if (me.isDestroyed) {
          return;
        }
        /**
         * Fires on successful request completion after data gets applied to the stores.
         * @event requestDone
         * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
         * @param {'sync'|'load'} requestType The request type (`sync` or `load`).
         * @param {Object} response The decoded server response object.
         * @param {Object} responseOptions The server response options.
         */
        me.trigger('requestDone', {
          requestType,
          response,
          responseOptions: fetchOptions
        });
        /**
         * Fires on successful {@link #function-load load request} completion after data gets loaded to the stores.
         * @event load
         * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
         * @param {Object} response The decoded server response object.
         * @param {Object} responseOptions The server response options.
         * @params {Object} options Options provided to the {@link #load} method.
         */
        /**
         * Fires on successful {@link #function-sync sync request} completion.
         * @event sync
         * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
         * @param {Object} response The decoded server response object.
         * @param {Object} responseOptions The server response options.
         */
        me.trigger(requestType, {
          response,
          responseOptions: fetchOptions,
          options
        });
        if (requestType === 'load' || !me.crudStoreHasChanges()) {
          /**
           * Fires when registered stores get into state when they don't have any
           * not persisted change. This happens after {@link #function-load} or {@link #function-sync} request
           * completion. Or this may happen after a record update which turns its fields back to their original state.
           *
           * ```javascript
           * crudManager.on('nochanges', function (crud) {
           *     // disable persist changes button when there is no changes
           *     saveButton.disable();
           * });
           * ```
           *
           * @event noChanges
           * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
           */
          me.trigger('noChanges');
          if (requestType === 'load') {
            me.emitCrudStoreEvents(request.pack.stores, 'afterRequest');
          }
        }
      }
      return response;
    }
    //endregion
    //region Changes tracking
    suspendChangesTracking() {
      this.crudIgnoreUpdates++;
    }
    resumeChangesTracking(skipChangeCheck) {
      if (this.crudIgnoreUpdates && ! --this.crudIgnoreUpdates && !skipChangeCheck) {
        this.onCrudStoreChange();
      }
    }
    get isBatchingChanges() {
      return this.crudIgnoreUpdates > 0;
    }
    /**
     * Returns `true` if any of registered stores (or some particular store) has non persisted changes.
     *
     * ```javascript
     * // if we have any unsaved changes
     * if (crudManager.crudStoreHasChanges()) {
     *     // persist them
     *     crudManager.sync();
     * // otherwise
     * } else {
     *     alert("There are no unsaved changes...");
     * }
     * ```
     *
     * @param {String|Core.data.Store} [storeId] The store identifier or store instance to check changes for.
     * If not specified then will check changes for all of the registered stores.
     * @returns {Boolean} `true` if there are not persisted changes.
     * @category CRUD
     */
    crudStoreHasChanges(storeId) {
      return storeId ? this.isCrudStoreDirty(this.getCrudStore(storeId)) : this.crudStores.some(config => this.isCrudStoreDirty(config.store));
    }
    isCrudStoreDirty(store) {
      return Boolean(store.changes);
    }
    //endregion
    //region Load
    doAutoLoad() {
      return this.load().catch(error => {});
    }
    emitCrudStoreEvents(stores, eventName, eventParams) {
      const event = {
        action: 'read' + eventName,
        ...eventParams
      };
      for (const store of this.crudStores) {
        if (stores.includes(store.storeId)) {
          store.store.trigger(eventName, event);
        }
      }
    }
    getLoadPackage(options) {
      const pack = {
          type: 'load',
          requestId: this.requestId
        },
        stores = this.crudStores,
        optionsCopy = Object.assign({}, options);
      // This is a special option which does not apply to a store.
      // It's used as options to the AjaxTransport#sendRequest method
      delete optionsCopy.request;
      pack.stores = stores.map(store => {
        var _store$store;
        const opts = optionsCopy === null || optionsCopy === void 0 ? void 0 : optionsCopy[store.storeId],
          pageSize = store.pageSize || ((_store$store = store.store) === null || _store$store === void 0 ? void 0 : _store$store.pageSize);
        if (opts || pageSize) {
          const params = Object.assign({
            storeId: store.storeId,
            page: 1
          }, opts);
          if (pageSize) {
            params.pageSize = pageSize;
          }
          store.currentPage = params.page;
          // Remove from common request options
          if (opts) {
            delete optionsCopy[store.storeId];
          }
          return params;
        }
        return store.storeId;
      });
      // Apply common request options
      Object.assign(pack, optionsCopy);
      return pack;
    }
    loadCrudStore(store, data, options) {
      const rows = data === null || data === void 0 ? void 0 : data.rows;
      if (options !== null && options !== void 0 && options.append || data !== null && data !== void 0 && data.append) {
        store.add(rows);
      } else {
        store.data = rows;
      }
      store.trigger('load', {
        data: rows
      });
    }
    loadDataToCrudStore(storeDesc, data, options) {
      const store = storeDesc.store,
        rows = data === null || data === void 0 ? void 0 : data.rows;
      store.__loading = true;
      if (rows) {
        this.loadCrudStore(store, data, options, storeDesc);
      }
      store.__loading = false;
    }
    /**
     * Loads data to the Crud Manager
     * @param {Object} response A simple object representing the data.
     * The object structure matches the decoded `load` response structure:
     *
     * ```js
     * // load static data into crudManager
     * crudManager.loadCrudManagerData({
     *     success   : true,
     *     resources : {
     *         rows : [
     *             { id : 1, name : 'John' },
     *             { id : 2, name : 'Abby' }
     *         ]
     *     }
     * });
     * ```
     * @param {Object} [options] Extra data loading options.
     * @category CRUD
     */
    loadCrudManagerData(response, options = {}) {
      // we don't want to react to store changes during loading of them
      this.suspendChangesTracking();
      // we load data to the stores in the order they're kept in this.stores array
      this.crudStores.forEach(storeDesc => {
        const storeId = storeDesc.storeId,
          data = response[storeId];
        if (data) {
          this.loadDataToCrudStore(storeDesc, data, options[storeId]);
        }
      });
      this.resumeChangesTracking(true);
    }
    /**
     * Returns true if the crud manager is currently loading data
     * @property {Boolean}
     * @readonly
     * @category CRUD
     */
    get isCrudManagerLoading() {
      return Boolean(this.activeRequests.load || this.applyingLoadResponse);
    }
    /**
     * Returns true if the crud manager is currently syncing data
     * @property {Boolean}
     * @readonly
     * @category CRUD
     */
    get isCrudManagerSyncing() {
      return Boolean(this.activeRequests.sync || this.applyingSyncResponse);
    }
    get isLoadingOrSyncing() {
      return Boolean(this.isCrudManagerLoading || this.isCrudManagerSyncing);
    }
    /**
     * Loads data to the stores registered in the crud manager. For example:
     *
     * ```javascript
     * crudManager.load(
     *     // here are request parameters
     *     {
     *         store1 : { append : true, page : 3, smth : 'foo' },
     *         store2 : { page : 2, bar : '!!!' }
     *     }
     * ).then(
     *     () => alert('OMG! It works!'),
     *     ({ response, cancelled }) => console.log(`Error: ${cancelled ? 'Cancelled' : response.message}`)
     * );
     * ```
     *
     * ** Note: ** If there is an incomplete load request in progress then system will try to cancel it by calling {@link #function-cancelRequest}.
     * @param {Object|String} [options] The request parameters or a URL.
     * @param {Object} [options.request] An object which contains options to merge
     * into the options which are passed to {@link Scheduler/crud/transport/AjaxTransport#function-sendRequest}.
     * ```javascript
     * {
     *     store1 : { page : 3, append : true, smth : 'foo' },
     *     store2 : { page : 2, bar : '!!!' },
     *     request : {
     *         params : {
     *             startDate : '2021-01-01'
     *         }
     *     }
     * },
     * ```
     *
     * Omitting request arg:
     * ```javascript
     * crudManager.load().then(
     *     () => alert('OMG! It works!'),
     *     ({ response, cancelled }) => console.log(`Error: ${cancelled ? 'Cancelled' : response.message}`)
     * );
     * ```
     *
     * When presented it should be an object where keys are store Ids and values are, in turn, objects
     * of parameters related to the corresponding store. These parameters will be transferred in each
     * store's entry in the `stores` property of the POST data.
     *
     * Additionally, for flat stores `append: true` can be specified to add loaded records to the existing records,
     * default is to remove corresponding store's existing records first.
     * **Please note** that for delta loading you can also use an {@link #config-trackResponseType alternative approach}.
     * @param {'sync'|'load'} [options.request.type] The request type. Either `load` or `sync`.
     * @param {String} [options.request.url] The URL for the request. Overrides the URL defined in the `transport`
     * object
     * @param {String} [options.request.data] The encoded _Crud Manager_ request data.
     * @param {Object} [options.request.params] An object specifying extra HTTP params to send with the request.
     * @param {Function} [options.request.success] A function to be started on successful request transferring.
     * @param {String} [options.request.success.rawResponse] `Response` object returned by the
     * [fetch api](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API).
     * @param {Function} [options.request.failure] A function to be started on request transfer failure.
     * @param {String} [options.request.failure.rawResponse] `Response` object returned by the
     * [fetch api](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API).
     * @param {Object} [options.request.thisObj] `this` reference for the above `success` and `failure` functions.
     * @returns {Promise} Promise, which is resolved if request was successful.
     * Both the resolve and reject functions are passed a `state` object. State object has following structure:
     *
     * ```
     * {
     *     cancelled       : Boolean, // **optional** flag, which is present when promise was rejected
     *     rawResponse     : String,  // raw response from ajax request, either response xml or text
     *     rawResponseText : String,  // raw response text as String from ajax request
     *     response        : Object,  // processed response in form of object
     *     options         : Object   // options, passed to load request
     * }
     * ```
     *
     * If promise was rejected by {@link #event-beforeLoad} event, `state` object will have the following structure:
     *
     * ```
     * {
     *     cancelled : true
     * }
     * ```
     * @category CRUD
     * @async
     */
    load(options) {
      if (typeof options === 'string') {
        options = {
          request: {
            url: options
          }
        };
      }
      const me = this,
        pack = me.getLoadPackage(options);
      me._autoLoadPromise = null;
      return new Promise((resolve, reject) => {
        /**
         * Fires before {@link #function-load load request} is sent. Return `false` to cancel load request.
         * @event beforeLoad
         * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
         * @param {Object} pack The data package which contains data for all stores managed by the crud manager.
         */
        if (me.trigger('beforeLoad', {
          pack
        }) !== false) {
          var _options;
          // if another load request is in progress let's cancel it
          const {
            load
          } = me.activeRequests;
          if (load) {
            me.cancelRequest(load.desc, load.reject);
            me.trigger('loadCanceled', {
              pack
            });
          }
          const request = Objects.assign({
            id: pack.requestId,
            data: me.encode(pack),
            type: 'load',
            success: me.onCrudRequestSuccess,
            failure: me.onCrudRequestFailure,
            thisObj: me
          }, (_options = options) === null || _options === void 0 ? void 0 : _options.request);
          me.activeRequests.load = {
            type: 'load',
            options,
            pack,
            resolve,
            reject(...args) {
              // sendRequest will start a fetch promise, which we cannot reject from here. In fact what we
              // need to do, is to make fetch.then() to not call any real handlers. Which is what we do here.
              request.success = request.failure = null;
              reject(...args);
            },
            id: pack.requestId,
            desc: me.sendRequest(request)
          };
          me.emitCrudStoreEvents(pack.stores, 'loadStart');
          me.trigger('loadStart', {
            pack
          });
        } else {
          /**
           * Fired after {@link #function-load load request} was canceled by some {@link #event-beforeLoad}
           * listener or due to incomplete prior load request.
           * @event loadCanceled
           * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
           * @param {Object} pack The data package which contains data for all stores managed by the crud
           * manager.
           */
          me.trigger('loadCanceled', {
            pack
          });
          reject({
            cancelled: true
          });
        }
      });
    }
    getActiveCrudManagerRequest(requestType) {
      let request = this.activeRequests[requestType];
      if (!request && this.trackResponseType) {
        request = Object.values(this.activeRequests)[0];
      }
      return request;
    }
    //endregion
    //region Changes (prepare, process, get)
    prepareAddedRecordData(record, storeInfo) {
      const me = this,
        {
          store
        } = storeInfo,
        {
          isTree
        } = store,
        phantomIdField = storeInfo.phantomIdField || me.phantomIdField,
        phantomParentIdField = storeInfo.phantomParentIdField || me.phantomParentIdField,
        subStoreFields = store.modelClass.allFields.filter(field => field.subStore),
        cls = record.constructor,
        data = Object.assign(record.persistableData, {
          [phantomIdField]: record.id
        });
      if (isTree) {
        const {
          parent
        } = record;
        if (parent && !parent.isRoot && parent.isPhantom) {
          data[phantomParentIdField] = parent.id;
        }
      }
      if (me.resetIdsBeforeSync) {
        ObjectHelper.deletePath(data, cls.idField);
      }
      // If we have store fields that should be persisted w/ Crud Manager protocol
      subStoreFields.forEach(field => {
        const subStore = record.get(field.name);
        if (subStore.allCount) {
          data[field.dataSource] = {
            added: subStore.getRange().map(record => me.prepareAddedRecordData(record, {
              store: subStore
            }))
          };
        }
      });
      return data;
    }
    prepareAdded(list, storeInfo) {
      return list.filter(record => record.isValid).map(record => this.prepareAddedRecordData(record, storeInfo));
    }
    prepareUpdated(list, storeInfo) {
      const {
          store
        } = storeInfo,
        {
          isTree
        } = store,
        writeAllFields = storeInfo.writeAllFields || storeInfo.writeAllFields !== false && this.writeAllFields,
        phantomParentIdField = storeInfo.phantomParentIdField || this.phantomParentIdField,
        subStoreFields = store.modelClass.allFields.filter(field => field.subStore);
      if (storeInfo.store.tree) {
        const rootNode = storeInfo.store.rootNode;
        list = list.filter(record => record !== rootNode);
      }
      return list.filter(record => record.isValid).reduce((data, record) => {
        let recordData;
        // write all fields
        if (writeAllFields) {
          recordData = record.persistableData;
        } else {
          recordData = record.modificationDataToWrite;
        }
        if (isTree) {
          const {
            parent
          } = record;
          if (parent && !parent.isRoot && parent.isPhantom) {
            recordData[phantomParentIdField] = parent.id;
          }
        }
        // If we have store fields that should be persisted w/ Crud Manager protocal
        subStoreFields.forEach(field => {
          const subStore = record.get(field.name);
          recordData[field.dataSource] = this.getCrudStoreChanges({
            store: subStore
          });
        });
        // recordData can be null
        if (!ObjectHelper.isEmpty(recordData)) {
          data.push(recordData);
        }
        return data;
      }, []);
    }
    prepareRemoved(list) {
      return list.map(record => {
        const cls = record.constructor;
        return ObjectHelper.setPath({}, cls.idField, record.id);
      });
    }
    getCrudStoreChanges(storeDescriptor) {
      const {
        store
      } = storeDescriptor;
      let {
          added = [],
          modified: updated = [],
          removed = []
        } = store.changes || {},
        result;
      if (added.length) added = this.prepareAdded(added, storeDescriptor);
      if (updated.length) updated = this.prepareUpdated(updated, storeDescriptor);
      if (removed.length) removed = this.prepareRemoved(removed);
      // if this store has changes
      if (added.length || updated.length || removed.length) {
        result = {};
        if (added.length) result.added = added;
        if (updated.length) result.updated = updated;
        if (removed.length) result.removed = removed;
      }
      return result;
    }
    getChangesetPackage() {
      const {
        changes
      } = this;
      return changes || this.forceSync ? {
        type: 'sync',
        requestId: this.requestId,
        revision: this.crudRevision,
        ...changes
      } : null;
    }
    //endregion
    //region Apply
    /**
     * Returns current changes as an object consisting of added/modified/removed arrays of records for every
     * managed store, keyed by each store's `id`. Returns `null` if no changes exist. Format:
     *
     * ```javascript
     * {
     *     resources : {
     *         added    : [{ name : 'New guy' }],
     *         modified : [{ id : 2, name : 'Mike' }],
     *         removed  : [{ id : 3 }]
     *     },
     *     events : {
     *         modified : [{  id : 12, name : 'Cool task' }]
     *     },
     *     ...
     * }
     * ```
     *
     * @property {Object}
     * @readonly
     * @category CRUD
     */
    get changes() {
      const data = {};
      this.crudStores.forEach(store => {
        const changes = this.getCrudStoreChanges(store);
        if (changes) {
          data[store.storeId] = changes;
        }
      });
      return Object.keys(data).length > 0 ? data : null;
    }
    getRowsToApplyChangesTo({
      store,
      storeId
    }, storeResponse, storePack) {
      var _rows, _removed;
      const me = this,
        {
          modelClass
        } = store,
        idDataSource = modelClass.idField,
        // request data
        {
          updated: requestUpdated,
          removed: requestRemoved
        } = storePack || {};
      let rows, removed, remote;
      // If the response contains the store section
      if (storeResponse) {
        var _storeResponse$rows, _storeResponse$remove;
        remote = true;
        const respondedIds = {};
        // responded record changes/removals
        rows = ((_storeResponse$rows = storeResponse.rows) === null || _storeResponse$rows === void 0 ? void 0 : _storeResponse$rows.slice()) || [];
        removed = ((_storeResponse$remove = storeResponse.removed) === null || _storeResponse$remove === void 0 ? void 0 : _storeResponse$remove.slice()) || [];
        // Collect hash w/ identifiers of responded records
        [...rows, ...removed].forEach(responseRecord => {
          const id = ObjectHelper.getPath(responseRecord, idDataSource);
          respondedIds[id] = true;
        });
        // If it's told to support providing server changes only in response
        // CrudManager should collect other records to commit from current requested data
        if (me.supportShortSyncResponse) {
          // append records requested to update (if not there already)
          requestUpdated === null || requestUpdated === void 0 ? void 0 : requestUpdated.forEach(data => {
            const id = ObjectHelper.getPath(data, idDataSource);
            // if response doesn't include
            if (!respondedIds[id]) {
              rows.push({
                [idDataSource]: id
              });
            }
          });
          // append records requested to remove (if not there already)
          requestRemoved === null || requestRemoved === void 0 ? void 0 : requestRemoved.forEach(data => {
            const id = ObjectHelper.getPath(data, idDataSource);
            // if response doesn't include
            if (!respondedIds[id]) {
              removed.push({
                [idDataSource]: id
              });
            }
          });
        }
      }
      // If there is no this store section we use records mentioned in the current request
      else if (requestUpdated || requestRemoved) {
        remote = false;
        rows = requestUpdated;
        removed = requestRemoved;
      }
      // return nullish "rows"/"removed" if no entries
      rows = (_rows = rows) !== null && _rows !== void 0 && _rows.length ? rows : null;
      removed = (_removed = removed) !== null && _removed !== void 0 && _removed.length ? removed : null;
      return {
        rows,
        removed,
        remote
      };
    }
    applyChangesToStore(storeDesc, storeResponse, storePack) {
      const me = this,
        phantomIdField = storeDesc.phantomIdField || me.phantomIdField,
        {
          store
        } = storeDesc,
        idField = store.modelClass.getFieldDataSource('id'),
        subStoreFields = store.modelClass.allFields.filter(field => field.subStore),
        // collect records we need to process
        {
          rows,
          removed,
          remote
        } = me.getRowsToApplyChangesTo(storeDesc, storeResponse, storePack),
        added = [],
        updated = [];
      // Convert to the { updated, added } format accepted by stores
      if (rows) {
        for (const data of rows) {
          // Existing records are updated
          if (store.getById(data[phantomIdField] ?? data[idField])) {
            updated.push(data);
          }
          // Others added
          else {
            added.push(data);
          }
        }
      }
      const extraLogEntries = [];
      // Handle sub-stores (if any)
      if (updated.length && subStoreFields.length) {
        updated.forEach(updateData => {
          var _storePack$added, _storePack$updated;
          const record = store.getById(updateData[phantomIdField] ?? updateData[idField]),
            // find the request portion related to the record
            recordRequest = ((_storePack$added = storePack.added) === null || _storePack$added === void 0 ? void 0 : _storePack$added.find(t => t[phantomIdField] == updateData[phantomIdField])) || ((_storePack$updated = storePack.updated) === null || _storePack$updated === void 0 ? void 0 : _storePack$updated.find(t => t[idField] == updateData[idField]));
          const extraLogInfo = {};
          subStoreFields.forEach(field => {
            const store = record.get(field.name);
            me.applyChangesToStore({
              store
            }, updateData[field.dataSource], recordRequest === null || recordRequest === void 0 ? void 0 : recordRequest[field.dataSource]);
            // We're putting the store field entry to the log
            // just to indicate the fact it was actually changed.
            // The value will not be used for comparison so we can use any.
            extraLogInfo[field.dataSource] = 'foo';
            delete updateData[field.dataSource];
          });
          extraLogEntries.push([record, extraLogInfo]);
        });
      }
      // process added/updated records
      const log = store.applyChangeset({
        removed,
        added,
        updated
      }, null, phantomIdField, remote, true);
      extraLogEntries.forEach(([record, logEntry]) => Object.assign(log.get(record.id), logEntry));
      return log;
    }
    applySyncResponse(response, request) {
      const me = this,
        stores = me.orderedCrudStores;
      me.applyingChangeset = me.applyingSyncResponse = true;
      me.suspendChangesTracking();
      for (const store of stores) {
        var _request$pack;
        me.applyChangesToStore(store, response[store.storeId], request === null || request === void 0 ? void 0 : (_request$pack = request.pack) === null || _request$pack === void 0 ? void 0 : _request$pack[store.storeId]);
      }
      me.resumeChangesTracking(true);
      me.applyingChangeset = me.applyingSyncResponse = false;
    }
    applyLoadResponse(response, options) {
      this.applyingLoadResponse = true;
      this.loadCrudManagerData(response, options);
      this.applyingLoadResponse = false;
    }
    async applyResponse(request, response, options) {
      const me = this,
        // in trackResponseType mode we check response type before deciding how to react on the response
        responseType = me.trackResponseType && response.type || request.type;
      switch (responseType) {
        case 'load':
          if (me.validateResponse) {
            me.validateLoadResponse(response);
          }
          me.applyLoadResponse(response, options);
          break;
        case 'sync':
          if (me.validateResponse) {
            me.validateSyncResponse(response, request);
          }
          me.applySyncResponse(response, request);
          break;
      }
    }
    /**
     * Applies a set of changes, as an object keyed by store id, to the affected stores. This function is intended
     * to use in apps that handle their own data syncing, it is not needed when using the CrudManager approach.
     *
     * Example of a changeset:
     * ```javascript
     * project.applyChangeset({
     *     events : {
     *         added : [
     *             { id : 10, name : 'Event 10', startDate : '2022-06-07' }
     *         ],
     *         updated : [
     *             { id : 5, name : 'Changed' }
     *         ],
     *         removed : [
     *             { id : 1 }
     *         ]
     *     },
     *     resources : { ... },
     *     ...
     * });
     * ```
     *
     * Optionally accepts a `transformFn` to convert an incoming changeset to the expected format.
     * See {@link Core/data/Store#function-applyChangeset} for more details.
     *
     * @param {Object} changes Changeset to apply, an object keyed by store id where each value follows the
     * format described in {@link Core/data/Store#function-applyChangeset}
     * @param {Function} [transformFn] Optional function used to preprocess a changeset per store in a different
     * format, should return an object with the format expected by {@link Core/data/Store#function-applyChangeset}
     * @param {String} [phantomIdField] Field used by the backend when communicating a record being assigned a
     * proper id instead of a phantom id
     */
    applyChangeset(changes, transformFn = null, phantomIdField, logChanges = false) {
      const me = this,
        log = logChanges ? new Map() : undefined;
      me.suspendAutoSync();
      me.suspendChangesTracking();
      for (const {
        store,
        phantomIdField
      } of me.orderedCrudStores) {
        if (changes[store.id]) {
          const storeLog = store.applyChangeset(changes[store.id], transformFn, phantomIdField || me.phantomIdField,
          // mark this changeset as remote to enforce it
          true, logChanges);
          if (storeLog) {
            log.set(store.id, storeLog);
          }
        }
      }
      me.resumeChangesTracking(true);
      me.resumeAutoSync(false);
      return log;
    }
    //endregion
    /**
     * Generates unique request identifier.
     * @internal
     * @template
     * @returns {Number} The request identifier.
     * @category CRUD
     */
    get requestId() {
      return Number.parseInt(`${Date.now()}${this._requestId++}`);
    }
    /**
     * Persists changes made on the registered stores to the server and/or receives changes made on the backend.
     * Usage:
     *
     * ```javascript
     * // persist and run a callback on request completion
     * crud.sync().then(
     *     () => console.log("Changes saved..."),
     *     ({ response, cancelled }) => console.log(`Error: ${cancelled ? 'Cancelled' : response.message}`)
     * );
     * ```
     *
     * ** Note: ** If there is an incomplete sync request in progress then system will queue the call and delay it
     * until previous request completion.
     * In this case {@link #event-syncDelayed} event will be fired.
     *
     * ** Note: ** Please take a look at {@link #config-autoSync} config. This option allows to persist changes
     * automatically after any data modification.
     *
     * ** Note: ** By default a sync request is only sent if there are any local {@link #property-changes}. To
     * always send a request when calling this function, configure {@link #config-forceSync} as `true`.
     *
     * @returns {Promise} Promise, which is resolved if request was successful.
     * Both the resolve and reject functions are passed a `state` object. State object has the following structure:
     * ```
     * {
     *     cancelled       : Boolean, // **optional** flag, which is present when promise was rejected
     *     rawResponse     : String,  // raw response from ajax request, either response xml or text
     *     rawResponseText : String,  // raw response text as String from ajax request
     *     response        : Object,  // processed response in form of object
     * }
     * ```
     * If promise was rejected by the {@link #event-beforeSync} event, `state` object will have this structure:
     * ```
     * {
     *     cancelled : true
     * }
     * ```
     * @category CRUD
     * @async
     */
    sync() {
      const me = this;
      // A direct call to sync cancels any outstanding autoSync
      me.clearTimeout('autoSync');
      if (me.activeRequests.sync) {
        // let's delay this call and start it only after server response
        /**
         * Fires after {@link #function-sync sync request} was delayed due to incomplete previous one.
         * @event syncDelayed
         * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
         * @param {Object} arguments The arguments of {@link #function-sync} call.
         */
        me.trigger('syncDelayed');
        // Queue sync request after current one
        return me.activeSyncPromise = me.activeSyncPromise.finally(() => me.sync());
      }
      // Store current request promise. While this one is pending, all following sync requests will create chain
      // of sequential promises
      return me.activeSyncPromise = new Promise((resolve, reject) => {
        // get current changes set package
        const pack = me.getChangesetPackage();
        // if no data to persist we resolve immediately
        if (!pack) {
          resolve(null);
          return;
        }
        /**
         * Fires before {@link #function-sync sync request} is sent. Return `false` to cancel sync request.
         *
         * ```javascript
         * crudManager.on('beforesync', function() {
         *     // cannot persist changes before at least one record is added
         *     // to the `someStore` store
         *     if (!someStore.getCount()) return false;
         * });
         * ```
         * @event beforeSync
         * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
         * @param {Object} pack The data package which contains data for all stores managed by the crud manager.
         */
        if (me.trigger('beforeSync', {
          pack
        }) !== false) {
          me.trigger('syncStart', {
            pack
          });
          // keep active request details
          me.activeRequests.sync = {
            type: 'sync',
            pack,
            resolve,
            reject,
            id: pack.requestId,
            desc: me.sendRequest({
              id: pack.requestId,
              data: me.encode(pack),
              type: 'sync',
              success: me.onCrudRequestSuccess,
              failure: me.onCrudRequestFailure,
              thisObj: me
            })
          };
        } else {
          /**
           * Fires after {@link #function-sync sync request} was canceled by some {@link #event-beforeSync} listener.
           * @event syncCanceled
           * @param {Scheduler.crud.AbstractCrudManager} source The CRUD manager.
           * @param {Object} pack The data package which contains data for all stores managed by the crud manager.
           */
          me.trigger('syncCanceled', {
            pack
          });
          reject({
            cancelled: true
          });
        }
      }).catch(error => {
        // If the request was not cancelled in beforeSync listener, forward the error so the user's `catch` handler can catch it
        if (error && !error.cancelled) {
          throw error;
        }
        // Pass the error object as a param to the next `then` chain
        return error;
      });
    }
    async onCrudRequestSuccess(rawResponse, fetchOptions, request) {
      var _request;
      const me = this,
        {
          type: requestType,
          id: requestId
        } = request;
      if (me.isDestroyed) return;
      let responseText = '';
      request = me.activeRequests[requestType];
      // we throw exception below to let events trigger first in internalOnResponse() call
      try {
        responseText = await rawResponse.text();
      } catch (e) {}
      // since we break the method w/ promises chain ..need to check if the instance is not destroyed in the meantime
      if (me.isDestroyed) return;
      // This situation should never occur.
      // In the load() method, if a load is called while there is a load
      // ongoing, the ongoing Transport request is cancelled and loadCanceled triggered.
      // But having got here, it's too late to cancel a Transport request, so
      // the operation is unregistered below.
      // In the sync() method, if a sync is called while there is a sync
      // ongoing, it waits until completion, before syncing.
      // The activeRequest for any operation should NEVER be able to be
      // replaced while this operation is ongoing, so this must be fatal.
      if (((_request = request) === null || _request === void 0 ? void 0 : _request.id) !== requestId) {
        throw new Error(`Interleaved ${requestType} operation detected`);
      }
      // Reset the active request info before we enter async code which could allow
      // application code to run which could potentially call another request.
      // It is too late for this request to be canceled - the activeRequest represented
      // the Transport object and that has completed now.
      me.activeRequests[requestType] = null;
      const response = await me.internalOnResponse(request, responseText, fetchOptions);
      // since we break the method w/ promises chain ..need to check if the instance is not destroyed in the meantime
      if (me.isDestroyed) return;
      if (!response || (me.skipSuccessProperty ? (response === null || response === void 0 ? void 0 : response.success) === false : !(response !== null && response !== void 0 && response.success))) {
        const error = {
          rawResponse,
          response,
          request
        };
        if (response !== null && response !== void 0 && response.message) {
          error.message = response.message;
        }
        request.reject(new CrudManagerRequestError(error));
      }
      // Successful request type done flag (this.crudLoaded or this.crudSynced)..
      me['crud' + StringHelper.capitalize(request.type) + 'ed'] = true;
      request.resolve({
        response,
        rawResponse,
        responseText,
        request
      });
    }
    async onCrudRequestFailure(rawResponse, fetchOptions, request) {
      var _fetchOptions$abortCo;
      const me = this;
      if (me.isDestroyed) return;
      request = me.activeRequests[request.type];
      const signal = fetchOptions === null || fetchOptions === void 0 ? void 0 : (_fetchOptions$abortCo = fetchOptions.abortController) === null || _fetchOptions$abortCo === void 0 ? void 0 : _fetchOptions$abortCo.signal,
        wasAborted = Boolean(signal === null || signal === void 0 ? void 0 : signal.aborted);
      if (!wasAborted) {
        let response,
          responseText = '';
        try {
          responseText = await rawResponse.text();
          response = me.decode(responseText);
        } catch (e) {}
        // since we break the method w/ promises chain ..need to check if the instance is not destroyed in the meantime
        if (me.isDestroyed) return;
        me.triggerFailedRequestEvents(request, response, responseText, fetchOptions);
        // since we break the method w/ promises chain ..need to check if the instance is not destroyed in the meantime
        if (me.isDestroyed) return;
        request.reject(new CrudManagerRequestError({
          rawResponse,
          request
        }));
      }
      // reset the active request info
      me.activeRequests[request.type] = null;
    }
    /**
     * Accepts all changes in all stores, resets the modification tracking:
     * * Clears change tracking for all records
     * * Clears added
     * * Clears modified
     * * Clears removed
     * Leaves the store in an "unmodified" state.
     * @category CRUD
     */
    acceptChanges() {
      this.crudStores.forEach(store => store.store.acceptChanges());
    }
    /**
     * Reverts all changes in all stores and re-inserts any records that were removed locally. Any new uncommitted
     * records will be removed.
     * @category CRUD
     */
    revertChanges() {
      // the method aliases revertCrudStoreChanges
      this.revertCrudStoreChanges();
    }
    revertCrudStoreChanges() {
      const {
        usesSingleAssignment
      } = this.eventStore;
      // Ignore assignment store if using single assignment, otherwise reverting changes will undo changes from
      // reverting the event store after reassignment
      this.orderedCrudStores.forEach(({
        store
      }) => (!store.isAssignmentStore || !usesSingleAssignment) && store.revertChanges());
    }
    /**
     * Removes all stores and cancels active requests.
     * @category CRUD
     * @internal
     */
    doDestroy() {
      const me = this,
        {
          load,
          sync
        } = me.activeRequests;
      load && me.cancelRequest(load.desc, load.reject);
      sync && me.cancelRequest(sync.desc, sync.reject);
      while (me.crudStores.length > 0) {
        me.removeCrudStore(me.crudStores[0]);
      }
      super.doDestroy && super.doDestroy();
    }
  };
});

/**
 * @module Scheduler/crud/transport/AjaxTransport
 */
/**
 * Implements data transferring functional that can be used for {@link Scheduler.crud.AbstractCrudManager} super classing.
 * Uses the fetch API for transport, https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API
 *
 * @example
 * // create a new CrudManager using AJAX as a transport system and JSON for encoding
 * class MyCrudManager extends AjaxTransport(JsonEncode(AbstractCrudManager)) {}
 *
 * @abstract
 * @mixin
 */
var AjaxTransport = (Target => class AjaxTransport extends (Target || Base) {
  static get $name() {
    return 'AjaxTransport';
  }
  /**
   * Configuration of the AJAX requests used by _Crud Manager_ to communicate with a server-side.
   *
   * ```javascript
   * transport : {
   *     load : {
   *         url       : 'http://mycool-server.com/load.php',
   *         // HTTP request parameter used to pass serialized "load"-requests
   *         paramName : 'data',
   *         // pass extra HTTP request parameter
   *         params    : {
   *             foo : 'bar'
   *         }
   *     },
   *     sync : {
   *         url     : 'http://mycool-server.com/sync.php',
   *         // specify Content-Type for requests
   *         headers : {
   *             'Content-Type' : 'application/json'
   *         }
   *     }
   * }
   *```
   * Since the class uses Fetch API you can use
   * any its [Request interface](https://developer.mozilla.org/en-US/docs/Web/API/Request) options:
   *
   * ```javascript
   * transport : {
   *     load : {
   *         url         : 'http://mycool-server.com/load.php',
   *         // HTTP request parameter used to pass serialized "load"-requests
   *         paramName   : 'data',
   *         // pass few Fetch API options
   *         method      : 'GET',
   *         credentials : 'include',
   *         cache       : 'no-cache'
   *     },
   *     sync : {
   *         url         : 'http://mycool-server.com/sync.php',
   *         // specify Content-Type for requests
   *         headers     : {
   *             'Content-Type' : 'application/json'
   *         },
   *         credentials : 'include'
   *     }
   * }
   *```
   *
   * An object where you can set the following possible properties:
   * @config {Object} transport
   * @property {Object} transport.load Load requests configuration:
   * @property {String} transport.load.url URL to request for data loading.
   * @property {String} [transport.load.method='GET'] HTTP method to be used for load requests.
   * @property {String} [transport.load.paramName='data'] Name of the parameter that will contain a serialized `load`
   * request. The value is mandatory for requests using `GET` method (default for `load`) so if the value is not
   * provided `data` string is used as default.
   * This value is optional for HTTP methods like `POST` and `PUT`, the request body will be used for data
   * transferring in these cases.
   * @property {Object} [transport.load.params] An object containing extra HTTP parameters to pass to the server when
   * sending a `load` request.
   *
   * ```javascript
   * transport : {
   *     load : {
   *         url       : 'http://mycool-server.com/load.php',
   *         // HTTP request parameter used to pass serialized "load"-requests
   *         paramName : 'data',
   *         // pass extra HTTP request parameter
   *         // so resulting URL will look like: http://mycool-server.com/load.php?userId=123456&data=...
   *         params    : {
   *             userId : '123456'
   *         }
   *     },
   *     ...
   * }
   * ```
   * @property {Object<String,String>} [transport.load.headers] An object containing headers to pass to each server request.
   *
   * ```javascript
   * transport : {
   *     load : {
   *         url       : 'http://mycool-server.com/load.php',
   *         // HTTP request parameter used to pass serialized "load"-requests
   *         paramName : 'data',
   *         // specify Content-Type for "load" requests
   *         headers   : {
   *             'Content-Type' : 'application/json'
   *         }
   *     },
   *     ...
   * }
   * ```
   * @property {Object} [transport.load.fetchOptions] **DEPRECATED:** Any Fetch API options can be simply defined on
   * the upper configuration level:
   * ```javascript
   * transport : {
   *     load : {
   *         url          : 'http://mycool-server.com/load.php',
   *         // HTTP request parameter used to pass serialized "load"-requests
   *         paramName    : 'data',
   *         // Fetch API options
   *         method       : 'GET',
   *         credentials  : 'include'
   *     },
   *     ...
   * }
   * ```
   * @property {Object} [transport.load.requestConfig] **DEPRECATED:** The config options can be defined on the upper
   * configuration level.
   * @property {Object} transport.sync Sync requests (`sync` in further text) configuration:
   * @property {String} transport.sync.url URL to request for `sync`.
   * @property {String} [transport.sync.method='POST'] HTTP request method to be used for `sync`.
   * @property {String} [transport.sync.paramName=undefined] Name of the parameter in which `sync` data will be
   * transferred. This value is optional for requests using methods like `POST` and `PUT`, the request body will be
   * used for data transferring in this case (default for `sync`). And the value is mandatory for requests using `GET`
   * method (if the value is not provided `data` string will be used as fallback).
   * @property {Object} [transport.sync.params] HTTP parameters to pass with an HTTP request handling `sync`.
   *
   * ```javascript
   * transport : {
   *     sync : {
   *         url    : 'http://mycool-server.com/sync.php',
   *         // extra HTTP request parameter
   *         params : {
   *             userId : '123456'
   *         }
   *     },
   *     ...
   * }
   * ```
   * @property {Object<String,String>} [transport.sync.headers] HTTP headers to pass with an HTTP request handling `sync`.
   *
   * ```javascript
   * transport : {
   *     sync : {
   *         url     : 'http://mycool-server.com/sync.php',
   *         // specify Content-Type for "sync" requests
   *         headers : {
   *             'Content-Type' : 'application/json'
   *         }
   *     },
   *     ...
   * }
   * ```
   * @property {Object} [transport.sync.fetchOptions] **DEPRECATED:** Any Fetch API options can be simply defined on
   * the upper configuration level:
   * ```javascript
   * transport : {
   *     sync : {
   *         url         : 'http://mycool-server.com/sync.php',
   *         credentials : 'include'
   *     },
   *     ...
   * }
   * ```
   * @property {Object} [transport.sync.requestConfig] **DEPRECATED:** The config options can be defined on the upper
   * configuration level.
   * @category CRUD
   */
  static get defaultMethod() {
    return {
      load: 'GET',
      sync: 'POST'
    };
  }
  /**
   * Cancels a sent request.
   * @param {Promise} requestPromise The Promise object wrapping the Request to be cancelled.
   * The _requestPromise_ is the value returned from the corresponding {@link #function-sendRequest} call.
   * @category CRUD
   */
  cancelRequest(requestPromise, reject) {
    var _requestPromise$abort;
    (_requestPromise$abort = requestPromise.abort) === null || _requestPromise$abort === void 0 ? void 0 : _requestPromise$abort.call(requestPromise);
    if (!this.isDestroying) {
      reject({
        cancelled: true
      });
    }
  }
  shouldUseBodyForRequestData(packCfg, method, paramName) {
    return !(method === 'HEAD' || method === 'GET') && !paramName;
  }
  /**
   * Sends a _Crud Manager_ request to the server.
   * @param {Object} request The request configuration object having following properties:
   * @param {'load'|'sync'} request.type The request type. Either `load` or `sync`.
   * @param {String} request.url The URL for the request. Overrides the URL defined in the `transport` object
   * @param {String} request.data The encoded _Crud Manager_ request data.
   * @param {Object} request.params An object specifying extra HTTP params to send with the request.
   * @param {Function} request.success A function to be started on successful request transferring.
   * @param {String} request.success.rawResponse `Response` object returned by the [fetch api](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API).
   * @param {Function} request.failure A function to be started on request transfer failure.
   * @param {String} request.failure.rawResponse `Response` object returned by the [fetch api](https://developer.mozilla.org/en-US/docs/Web/API/Fetch_API).
   * @param {Object} request.thisObj `this` reference for the above `success` and `failure` functions.
   * @returns {Promise} The fetch Promise object.
   * @fires beforeSend
   * @async
   * @category CRUD
   */
  sendRequest(request) {
    const me = this,
      {
        data
      } = request,
      transportConfig = me.transport[request.type] || {},
      // clone parameters defined for this type of request
      requestConfig = Objects.assign({}, transportConfig, transportConfig.requestConfig);
    if (request.url) {
      requestConfig.url = request.url;
    }
    requestConfig.method = requestConfig.method || AjaxTransport.defaultMethod[request.type];
    requestConfig.params = Objects.assign(requestConfig.params || {}, request.params);
    let {
      paramName
    } = requestConfig;
    // transfer package in the request body for some types of HTTP requests
    if (me.shouldUseBodyForRequestData(transportConfig, requestConfig.method, paramName)) {
      requestConfig.body = data;
      // for requests having body we set Content-Type to 'application/json' by default
      requestConfig.headers = requestConfig.headers || {};
      requestConfig.headers['Content-Type'] = requestConfig.headers['Content-Type'] || 'application/json';
    } else {
      // when we don't use body paramName is mandatory so fallback to 'data' as name
      paramName = paramName || 'data';
      requestConfig.params[paramName] = data;
    }
    if (!requestConfig.url) {
      throw new Error('Trying to request without URL specified');
    }
    // sanitize request config
    delete requestConfig.requestConfig;
    delete requestConfig.paramName;
    let ajaxPromise, resultPromise;
    function performSend() {
      // AjaxHelper.fetch call it "queryParams"
      requestConfig.queryParams = requestConfig.params;
      delete requestConfig.params;
      let cancelled = false;
      const fetchOptions = Objects.assign({}, requestConfig, requestConfig.fetchOptions);
      ajaxPromise = AjaxHelper.fetch(requestConfig.url, fetchOptions);
      return ajaxPromise.catch(error => {
        var _me$trigger, _fetchOptions$abortCo;
        ajaxPromise.done = true;
        (_me$trigger = me.trigger) === null || _me$trigger === void 0 ? void 0 : _me$trigger.call(me, 'responseReceived', {
          success: false
        });
        const signal = (_fetchOptions$abortCo = fetchOptions.abortController) === null || _fetchOptions$abortCo === void 0 ? void 0 : _fetchOptions$abortCo.signal;
        if (signal) {
          cancelled = signal.aborted;
          if (!cancelled) {
            console.warn(error);
          }
        }
        return {
          error,
          cancelled
        };
      }).then(response => {
        var _me$trigger2;
        ajaxPromise.done = true;
        (_me$trigger2 = me.trigger) === null || _me$trigger2 === void 0 ? void 0 : _me$trigger2.call(me, 'responseReceived', {
          success: Boolean(response === null || response === void 0 ? void 0 : response.ok)
        });
        const callback = response !== null && response !== void 0 && response.ok ? request.success : request.failure;
        return callback === null || callback === void 0 ? void 0 : callback.call(request.thisObj || me, response, fetchOptions, request);
      });
    }
    /**
     * Fires before a request is sent to the server.
     *
     * ```javascript
     * crudManager.on('beforeSend', function ({ params, type }) {
     *     // let's set "sync" request parameters
     *     if (type == 'sync') {
     *         // dynamically depending on "flag" value
     *         if (flag) {
     *             params.foo = 'bar';
     *         }
     *         else {
     *             params.foo = 'smth';
     *         }
     *     }
     * });
     * ```
     * @event beforeSend
     * @param {Scheduler.crud.AbstractCrudManager} crudManager The CRUD manager.
     * @param {Object} params HTTP request params to be passed in the request URL.
     * @param {'sync'|'load'} requestType CrudManager request type (`load`/`sync`)
     * @param {Object} requestConfig Configuration object for Ajax request call
     * @async
     */
    const beforeSendResult = me.trigger('beforeSend', {
      params: requestConfig.params,
      requestType: request.type,
      requestConfig,
      config: request
    });
    if (Objects.isPromise(beforeSendResult)) {
      resultPromise = beforeSendResult.then(performSend);
    } else {
      resultPromise = performSend();
    }
    resultPromise.abort = () => {
      if (!ajaxPromise.done) {
        var _ajaxPromise$abort, _ajaxPromise;
        (_ajaxPromise$abort = (_ajaxPromise = ajaxPromise).abort) === null || _ajaxPromise$abort === void 0 ? void 0 : _ajaxPromise$abort.call(_ajaxPromise);
      }
    };
    return resultPromise;
  }
});

/**
 * @module Scheduler/crud/encoder/JsonEncoder
 */
/**
 * Implements data encoding functional that should be mixed to a {@link Scheduler.crud.AbstractCrudManager} sub-class.
 * Uses _JSON_ as an encoding system.
 *
 * @example
 * // create a new CrudManager using AJAX as a transport system and JSON for encoding
 * class MyCrudManager extends JsonEncode(AjaxTransport(AbstractCrudManager)) {}
 *
 * @mixin
 */
var JsonEncoder = (Target => class JsonEncoder extends (Target || Base) {
  static get $name() {
    return 'JsonEncoder';
  }
  static get defaultConfig() {
    return {
      /**
       * Configuration of the JSON encoder used by the _Crud Manager_.
       *
       * @config {Object}
       * @property {Object} encoder.requestData Static data to send with the data request.
       *
       * ```js
       * new CrudManager({
       *     // add static "foo" property to all requests data
       *     encoder : {
       *         requestData : {
       *             foo : 'Bar'
       *         }
       *     },
       *     ...
       * });
       * ```
       *
       * The above snippet will result adding "foo" property to all requests data:
       *
       * ```json
       *     {
       *         "requestId"   : 756,
       *         "type"        : "load",
       *
       *         "foo"         : "Bar",
       *
       *         "stores"      : [
       *             ...
       * ```
       * @category CRUD
       */
      encoder: {}
    };
  }
  /**
   * Encodes a request object to _JSON_ encoded string. If encoding fails (due to circular structure), it returns null.
   * Supposed to be overridden in case data provided by the _Crud Manager_ has to be transformed into format requested by server.
   * @param {Object} requestData The request to encode.
   * @returns {String} The encoded request.
   * @category CRUD
   */
  encode(requestData) {
    var _this$encoder;
    requestData = Object.assign({}, (_this$encoder = this.encoder) === null || _this$encoder === void 0 ? void 0 : _this$encoder.requestData, requestData);
    return StringHelper.safeJsonStringify(requestData);
  }
  /**
   * Decodes (parses) a _JSON_ response string to an object. If parsing fails, it returns null.
   * Supposed to be overridden in case data provided by server has to be transformed into format requested by the _Crud Manager_.
   * @param {String} responseText The response text to decode.
   * @returns {Object} The decoded response.
   * @category CRUD
   */
  decode(responseText) {
    return StringHelper.safeJsonParse(responseText);
  }
});

/**
 * @module Scheduler/crud/mixin/CrudManagerView
 */
/**
 * Mixin to track Crud Manager requests to the server and mask the view during them. For masking, it
 * uses the {@link Core.mixin.LoadMaskable#config-loadMask} and {@link Core.mixin.LoadMaskable#config-syncMask}
 * properties.
 *
 * @mixin
 * @extends Core/mixin/LoadMaskable
 */
var CrudManagerView = (Target => class CrudManagerView extends Target.mixin(LoadMaskable) {
  static get $name() {
    return 'CrudManagerView';
  }
  static config = {
    clearMaskDelay: null,
    // Test environment may be in a poll wait for mask to disappear.
    // Hiding the mask immediately, before the load sequence ends releases it too early
    testConfig: {
      clearMaskDelay: 0
    }
  };
  //region Init
  afterConstruct() {
    super.afterConstruct();
    const {
      crudManager,
      project
    } = this;
    if (this.loadMask && (crudManager || project).isCrudManagerLoading) {
      // Show loadMask if crud manager is already loading
      this.onCrudManagerLoadStart();
    }
  }
  //endregion
  /**
   * Applies the {@link Scheduler.crud.mixin.CrudManagerView#config-syncMask} as the
   * {@link Core.widget.Widget#config-masked mask} for this widget.
   * @internal
   */
  applySyncMask() {
    const {
      syncMask
    } = this;
    if (syncMask) {
      this.masked = Mask.mergeConfigs(this.loadMaskDefaults, syncMask);
    }
  }
  /**
   * Hooks up crud manager listeners
   * @private
   * @category Store
   */
  bindCrudManager(crudManager) {
    this.detachListeners('crudManager');
    crudManager === null || crudManager === void 0 ? void 0 : crudManager.ion({
      name: 'crudManager',
      loadStart: 'onCrudManagerLoadStart',
      beforeSend: 'onCrudManagerBeforeSend',
      load: 'onCrudManagerLoad',
      loadCanceled: 'onCrudManagerLoadCanceled',
      syncStart: 'onCrudManagerSyncStart',
      beforeApplyResponse: 'onCrudManagerBeforeApplyResponse',
      applyResponse: 'onCrudManagerApplyResponse',
      sync: 'onCrudManagerSync',
      syncCanceled: 'onCrudManagerSyncCanceled',
      requestFail: 'onCrudManagerRequestFail',
      responseReceived: 'onAjaxTransportResponseReceived',
      thisObj: this
    });
  }
  onCrudManagerBeforeSend({
    params
  }) {
    var _this$applyStartEndPa;
    (_this$applyStartEndPa = this.applyStartEndParameters) === null || _this$applyStartEndPa === void 0 ? void 0 : _this$applyStartEndPa.call(this, params);
  }
  onCrudManagerLoadStart() {
    var _this$toggleEmptyText;
    // Show loadMask before crud manager starts loading
    this.applyLoadMask();
    (_this$toggleEmptyText = this.toggleEmptyText) === null || _this$toggleEmptyText === void 0 ? void 0 : _this$toggleEmptyText.call(this);
  }
  onCrudManagerSyncStart() {
    this.applySyncMask();
  }
  onCrudManagerBeforeApplyResponse() {
    var _this$suspendRefresh;
    // Prevent redrawing for each applied change, instead do it once after all changes are applied
    // (TaskBoard does not have suspendRefresh/resumeRefresh, it already updates on a buffer so not needed)
    (_this$suspendRefresh = this.suspendRefresh) === null || _this$suspendRefresh === void 0 ? void 0 : _this$suspendRefresh.call(this);
  }
  onCrudManagerApplyResponse() {
    var _this$resumeRefresh;
    // Repaint rows once after applying changes
    (_this$resumeRefresh = this.resumeRefresh) === null || _this$resumeRefresh === void 0 ? void 0 : _this$resumeRefresh.call(this, true);
  }
  onCrudManagerRequestFinalize(successful = true, requestType, response) {
    const me = this;
    if (successful) {
      var _me$toggleEmptyText;
      (_me$toggleEmptyText = me.toggleEmptyText) === null || _me$toggleEmptyText === void 0 ? void 0 : _me$toggleEmptyText.call(me);
    } else {
      if (!me.masked) {
        me.applyLoadMask();
      }
      me.applyMaskError(`<div class="b-grid-load-failure">
                    <div class="b-grid-load-fail">${me.L(`L{GridBase.${requestType}FailedMessage}`)}</div>
                    ${response && response.message ? `<div class="b-grid-load-fail">${me.L('L{CrudManagerView.serverResponseLabel}')} ${response.message}</div>` : ''}
                </div>`);
    }
  }
  onCrudManagerLoadCanceled() {
    this.onCrudManagerRequestFinalize(true, 'load');
  }
  onCrudManagerSyncCanceled() {
    this.onCrudManagerRequestFinalize(true, 'sync');
  }
  onCrudManagerLoad() {
    this.onCrudManagerRequestFinalize(true, 'load');
  }
  onCrudManagerSync() {
    this.onCrudManagerRequestFinalize(true, 'sync');
  }
  onCrudManagerRequestFail({
    requestType,
    response
  }) {
    this.onCrudManagerRequestFinalize(false, requestType, response);
  }
  onAjaxTransportResponseReceived() {
    const me = this;
    if (me.clearMaskDelay != null) {
      me.setTimeout(() => me.masked = null, me.clearMaskDelay);
    } else {
      me.masked = null;
    }
  }
  get widgetClass() {}
});

/**
 * @module Scheduler/data/mixin/PartOfProject
 */
/**
 * This is a mixin, included in all models and stores of the Scheduler project. It provides a common API for accessing
 * all stores of the project.
 *
 * @mixin
 */
var PartOfProject = (Target => class PartOfProject extends (Target || Base) {
  /**
   * Returns the project this entity belongs to.
   *
   * @member {Scheduler.model.ProjectModel} project
   * @readonly
   * @category Project
   */
  /**
   * Returns the event store of the project this entity belongs to.
   *
   * @member {Scheduler.data.EventStore} eventStore
   * @readonly
   * @category Project
   */
  /**
   * Returns the dependency store of the project this entity belongs to.
   *
   * @member {Scheduler.data.DependencyStore} dependencyStore
   * @readonly
   * @category Project
   */
  /**
   * Returns the assignment store of the project this entity belongs to.
   *
   * @member {Scheduler.data.AssignmentStore} assignmentStore
   * @readonly
   * @category Project
   */
  /**
   * Returns the resource store of the project this entity belongs to.
   *
   * @member {Scheduler.data.ResourceStore} resourceStore
   * @readonly
   * @category Project
   */
  static get $name() {
    return 'PartOfProject';
  }
});

/**
 * @module Scheduler/model/mixin/AssignmentModelMixin
 */
/**
 * Mixin that holds configuration shared between assignments in Scheduler and Scheduler Pro.
 * @mixin
 */
var AssignmentModelMixin = (Target => class AssignmentModelMixin extends Target {
  static get $name() {
    return 'AssignmentModelMixin';
  }
  /**
   * Set value for the specified field(s), triggering engine calculations immediately. See
   * {@link Core.data.Model#function-set Model#set()} for arguments.
   *
   * ```javascript
   * assignment.set('resourceId', 2);
   * // assignment.resource is not yet resolved
   *
   * await assignment.setAsync('resourceId', 2);
   * // assignment.resource is resolved
   * ```
   *
   * @param {String|Object} field The field to set value for, or an object with multiple values to set in one call
   * @param {*} value Value to set
   * @param {Boolean} [silent=false] Set to true to not trigger events
   * automatically.
   * @function setAsync
   * @category Editing
   * @async
   */
  //region Fields
  static get fields() {
    return [
    /**
     * Id for the resource to assign to
     * @field {String|Number} resourceId
     * @category Common
     */
    'resourceId',
    /**
     * Id for the event to assign
     * @field {String|Number} eventId
     * @category Common
     */
    'eventId',
    /**
     * Specify `false` to opt out of drawing dependencies from/to this assignment
     * @field {Boolean} drawDependencies
     * @category Common
     */
    {
      name: 'drawDependencies',
      type: 'boolean'
    }, 'event', 'resource'];
  }
  //endregion
  construct(data, ...args) {
    data = data || {};
    const {
        fieldMap
      } = this,
      eventIdField = fieldMap.eventId.dataSource,
      resourceIdField = fieldMap.resourceId.dataSource,
      eventField = fieldMap.event.dataSource,
      resourceField = fieldMap.resource.dataSource,
      eventId = data[eventIdField],
      resourceId = data[resourceIdField],
      event = data[eventField],
      resource = data[resourceField];
    // Engine expects event and resource, not eventId and resourceId. We need to support both
    if (eventId != null) {
      data[eventField] = eventId;
    } else if (event != null) {
      data[eventIdField] = event.isModel ? event.id : event;
    }
    if (resourceId != null) {
      data[resourceField] = resourceId;
    } else if (resource != null) {
      data[resourceIdField] = resource.isModel ? resource.id : resource;
    }
    super.construct(data, ...args);
  }
  //region Event & resource
  /**
   * A key made up from the event id and the id of the resource assigned to.
   * @property eventResourceKey
   * @readonly
   * @internal
   */
  get eventResourceKey() {
    return this.buildEventResourceKey(this.event, this.resource);
  }
  buildEventResourceKey(event, resource) {
    let eventKey, resourceKey;
    if (event) {
      eventKey = event.isModel ? event.id : event;
    } else {
      eventKey = this.internalId;
    }
    if (resource) {
      resourceKey = resource.isModel ? resource.id : resource;
    } else {
      resourceKey = this.internalId;
    }
    return `${eventKey}-${resourceKey}`;
  }
  buildIndexKey({
    event,
    resource
  }) {
    return this.buildEventResourceKey(event, resource);
  }
  set(field, value, ...args) {
    const toSet = this.fieldToKeys(field, value);
    // If resource was set, store its id as resourceId and announce it
    if ('resource' in toSet) {
      var _toSet$resource;
      if (((_toSet$resource = toSet.resource) === null || _toSet$resource === void 0 ? void 0 : _toSet$resource.id) !== undefined) {
        toSet.resourceId = toSet.resource.id;
      }
    }
    // If resourceId was set, we need to replace resource to have it re-referenced in engine
    else if ('resourceId' in toSet && this.constructor.isProAssignmentModel) {
      toSet.resource = toSet.resourceId;
    }
    // Same for event
    if ('event' in toSet) {
      var _toSet$event;
      if (((_toSet$event = toSet.event) === null || _toSet$event === void 0 ? void 0 : _toSet$event.id) !== undefined) {
        toSet.eventId = toSet.event.id;
      }
    } else if ('eventId' in toSet && this.constructor.isProAssignmentModel) {
      toSet.event = toSet.eventId;
    }
    return super.set(toSet, null, ...args);
  }
  afterChange(toSet, wasSet, silent, fromRelationUpdate, skipAccessors) {
    var _me$resource, _me$project;
    const me = this;
    // Make sure Core Engine implementation notices "resourceId" change
    // and updates "resource" field value respectively
    if (!me.constructor.isProAssignmentModel && wasSet !== null && wasSet !== void 0 && wasSet.resourceId && ((_me$resource = me.resource) === null || _me$resource === void 0 ? void 0 : _me$resource.id) !== wasSet.resourceId.value) {
      me.resource = wasSet.resourceId.value;
    }
    // If that's non-Core Engine and we have "eventId" indicated as just set
    // and there was also "event" passed to set but it was not set
    // and if that happens when propagating after sync-response applied
    // - this means an event has changed its identifier
    // (this fixes 11_backend.t.js in the Gantt)
    else if (me.constructor.isProAssignmentModel && (_me$project = me.project) !== null && _me$project !== void 0 && _me$project.propagatingSyncChanges && wasSet !== null && wasSet !== void 0 && wasSet.eventId && !(wasSet !== null && wasSet !== void 0 && wasSet.event) && (toSet.event.value === wasSet.eventId.value || toSet.event.value.id === wasSet.eventId.value)) {
      // sanitize both wasSet and meta.modified
      delete wasSet.eventId;
      delete me.meta.modified.eventId;
    }
    return super.afterChange(...arguments);
  }
  // Settings resourceId relays to `resource`. Underlying data will be updated in `afterChange()` above
  set resourceId(value) {
    const {
      resource
    } = this;
    // When assigning a new id to a resource, it will update the resourceId of the assignment. But the assignments
    // resource is still the same so we need to announce here
    if (resource !== null && resource !== void 0 && resource.isModel && resource.id === value) {
      this.set('resourceId', value);
    } else {
      this.resource = value;
    }
  }
  get resourceId() {
    var _this$resource;
    // If assigned using `resource` and not `resourceId` there will be no resourceId
    return ((_this$resource = this.resource) === null || _this$resource === void 0 ? void 0 : _this$resource.id) ?? this.get('resourceId');
  }
  // Same for event as for resourceId
  set eventId(value) {
    const {
      event
    } = this;
    // When assigning a new id to an event, it will update the eventId of the assignment. But the assignments
    // event is still the same so we need to announce here
    if (event !== null && event !== void 0 && event.isModel && event.id === value) {
      this.set('eventId', value);
    } else {
      this.event = value;
    }
  }
  get eventId() {
    var _this$event;
    // If assigned using `event` and not `eventId` there will be no eventId
    return ((_this$event = this.event) === null || _this$event === void 0 ? void 0 : _this$event.id) ?? this.get('eventId');
  }
  /**
   * Convenience property to get the name of the associated event.
   * @property {String}
   * @readonly
   */
  get eventName() {
    var _this$event2;
    return (_this$event2 = this.event) === null || _this$event2 === void 0 ? void 0 : _this$event2.name;
  }
  /**
   * Convenience property to get the name of the associated resource.
   * @property {String}
   * @readonly
   */
  get resourceName() {
    var _this$resource2;
    return (_this$resource2 = this.resource) === null || _this$resource2 === void 0 ? void 0 : _this$resource2.name;
  }
  /**
   * Returns the resource associated with this assignment.
   *
   * @returns {Scheduler.model.ResourceModel} Instance of resource
   */
  getResource() {
    return this.resource;
  }
  //endregion
  // Convenience getter to not have to check `instanceof AssignmentModel`
  get isAssignment() {
    return true;
  }
  /**
   * Returns true if the Assignment can be persisted (e.g. task and resource are not 'phantoms')
   *
   * @property {Boolean}
   */
  get isPersistable() {
    var _this$event3;
    const {
        event,
        resource,
        unjoinedStores,
        assignmentStore
      } = this,
      crudManager = assignmentStore === null || assignmentStore === void 0 ? void 0 : assignmentStore.crudManager;
    let result;
    if (assignmentStore) {
      // If the owning event is not persistable, this assignment is also not persistable.
      // if crud manager is used it can deal with phantom event/resource since it persists all records in one batch
      // if no crud manager used we have to wait till event/resource are persisted
      result = this.isValid && event.isPersistable && (crudManager || !event.hasGeneratedId && !resource.hasGeneratedId);
    }
    // if we remove the record
    else {
      result = !this.isPhantom && Boolean(unjoinedStores[0]);
    }
    // Records not yet fully created cannot be persisted
    return result && super.isPersistable && !((_this$event3 = this.event) !== null && _this$event3 !== void 0 && _this$event3.isCreating);
  }
  get isValid() {
    return this.resource != null && this.event != null;
  }
  /**
   * Returns a textual representation of this assignment (e.g. Mike 50%).
   * @returns {String}
   */
  toString() {
    if (this.resourceName) {
      return `${this.resourceName} ${Math.round(this.units)}%`;
    }
    return '';
  }
  //region STM hooks
  shouldRecordFieldChange(fieldName, oldValue, newValue) {
    if (!super.shouldRecordFieldChange(fieldName, oldValue, newValue)) {
      return false;
    }
    if (fieldName === 'event' || fieldName === 'eventId') {
      var _this$project;
      const eventStore = (_this$project = this.project) === null || _this$project === void 0 ? void 0 : _this$project.eventStore;
      if (eventStore && eventStore.oldIdMap[oldValue] === eventStore.getById(newValue)) {
        return false;
      }
    }
    if (fieldName === 'resource' || fieldName === 'resourceId') {
      var _this$project2;
      const resourceStore = (_this$project2 = this.project) === null || _this$project2 === void 0 ? void 0 : _this$project2.resourceStore;
      if (resourceStore && resourceStore.oldIdMap[oldValue] === resourceStore.getById(newValue)) {
        return false;
      }
    }
    return true;
  }
  //endregion
});

const EngineMixin$8 = CoreAssignmentMixin;
/**
 * @module Scheduler/model/AssignmentModel
 */
/**
 * This model represents a single assignment of a resource to an event in the scheduler, usually added to a
 * {@link Scheduler.data.AssignmentStore}.
 *
 * It is a subclass of the {@link Core.data.Model} class. Please refer to the documentation for that class to become
 * familiar with the base interface of this class.
 *
 * ## Fields and references
 *
 * An Assignment has the following fields:
 * - `id` - The id of the assignment
 * - `resourceId` - The id of the resource assigned (optionally replaced with `resource` for load)
 * - `eventId` - The id of the event to which the resource is assigned (optionally replaced with `event` for load)
 *
 * The data source for these fields can be customized by subclassing this class:
 *
 * ```javascript
 * class MyAssignment extends AssignmentModel {
 *   static get fields() {
 *       return [
 *          { name : 'resourceId', dataSource : 'linkedResource' }
 *       ];
 *   }
 * }
 * ```
 *
 * After load and project normalization, these references are accessible (assuming their respective stores are loaded):
 * - `event` - The linked event record
 * - `resource` - The linked resource record
 *
 * ## Async resolving of references
 *
 * As described above, an assignment links an event to a resource. It holds references to an event record and a resource
 * record. These references are populated async, using the calculation engine of the project that the assignment via
 * its store is a part of. Because of this asyncness, references cannot be used immediately after modifications:
 *
 * ```javascript
 * assignment.resourceId = 2;
 * // assignment.resource is not yet available
 * ```
 *
 * To make sure references are updated, wait for calculations to finish:
 *
 * ```javascript
 * assignment.resourceId = 2;
 * await assignment.project.commitAsync();
 * // assignment.resource is available
 * ```
 *
 * As an alternative, you can also use `setAsync()` to trigger calculations directly after the change:
 *
 * ```javascript
 * await assignment.setAsync({ resourceId : 2});
 * // assignment.resource is available
 * ```
 *
 * @extends Core/data/Model
 * @mixes Scheduler/model/mixin/AssignmentModelMixin
 * @uninherit Core/data/mixin/TreeNode
 */
class AssignmentModel extends AssignmentModelMixin(PartOfProject(EngineMixin$8.derive(Model))) {
  // NOTE: Leave field defs at top to be picked up by jsdoc
  /**
   * Id for event to assign. Can be used as an alternative to `eventId`, but please note that after
   * load it will be populated with the actual event and not its id. This field is not persistable.
   * @field {Scheduler.model.EventModel} event
   * @accepts {String|Number|Scheduler.model.EventModel}
   * @category Common
   */
  /**
   * Id for resource to assign to. Can be used as an alternative to `resourceId`, but please note that after
   * load it will be populated with the actual resource and not its id. This field is not persistable.
   * @field {Scheduler.model.ResourceModel} resource
   * @accepts {String|Number|Scheduler.model.ResourceModel}
   * @category Common
   */
  static get $name() {
    return 'AssignmentModel';
  }
}
AssignmentModel.exposeProperties();
AssignmentModel._$name = 'AssignmentModel';

/**
 * @module Scheduler/data/mixin/AssignmentStoreMixin
 */
/**
 * This is a mixin, containing functionality related to managing assignments.
 *
 * It is consumed by the regular {@link Scheduler.data.AssignmentStore} class and Scheduler Pros counterpart.
 *
 * @mixin
 */
var AssignmentStoreMixin = (Target => class AssignmentStoreMixin extends Target {
  static get $name() {
    return 'AssignmentStoreMixin';
  }
  /**
   * Add assignments to the store.
   *
   * NOTE: References (event, resource) on the assignments are determined async by a calculation engine. Thus they
   * cannot be directly accessed after using this function.
   *
   * For example:
   *
   * ```javascript
   * const [assignment] = assignmentStore.add({ eventId, resourceId });
   * // assignment.event is not yet available
   * ```
   *
   * To guarantee references are set up, wait for calculations for finish:
   *
   * ```javascript
   * const [assignment] = assignmentStore.add({ eventId, resourceId });
   * await assignmentStore.project.commitAsync();
   * // assignment.event is available (assuming EventStore is loaded and so on)
   * ```
   *
   * Alternatively use `addAsync()` instead:
   *
   * ```javascript
   * const [assignment] = await assignmentStore.addAsync({ eventId, resourceId });
   * // assignment.event is available (assuming EventStore is loaded and so on)
   * ```
   *
   * @param {Scheduler.model.AssignmentModel|Scheduler.model.AssignmentModel[]|AssignmentModelConfig|AssignmentModelConfig[]} records
   * Array of records/data or a single record/data to add to store
   * @param {Boolean} [silent] Specify `true` to suppress events
   * @returns {Scheduler.model.AssignmentModel[]} Added records
   * @function add
   * @category CRUD
   */
  /**
   * Add assignments to the store and triggers calculations directly after. Await this function to have up to date
   * references on the added assignments.
   *
   * ```javascript
   * const [assignment] = await assignmentStore.addAsync({ eventId, resourceId });
   * // assignment.event is available (assuming EventStore is loaded and so on)
   * ```
   *
   * @param {Scheduler.model.AssignmentModel|Scheduler.model.AssignmentModel[]|AssignmentModelConfig|AssignmentModelConfig[]} records
   * Array of records/data or a single record/data to add to store
   * @param {Boolean} [silent] Specify `true` to suppress events
   * @returns {Scheduler.model.AssignmentModel[]} Added records
   * @function addAsync
   * @category CRUD
   * @async
   */
  /**
   * Applies a new dataset to the AssignmentStore. Use it to plug externally fetched data into the store.
   *
   * NOTE: References (assignments, resources) on the assignments are determined async by a calculation engine. Thus
   * they cannot be directly accessed after assigning the new dataset.
   *
   * For example:
   *
   * ```javascript
   * assignmentStore.data = [{ eventId, resourceId }];
   * // assignmentStore.first.event is not yet available
   * ```
   *
   * To guarantee references are available, wait for calculations for finish:
   *
   * ```javascript
   * assignmentStore.data = [{ eventId, resourceId  }];
   * await assignmentStore.project.commitAsync();
   * // assignmentStore.first.event is available
   * ```
   *
   * Alternatively use `loadDataAsync()` instead:
   *
   * ```javascript
   * await assignmentStore.loadDataAsync([{ eventId, resourceId }]);
   * // assignmentStore.first.event is available
   * ```
   *
   * @member {AssignmentModelConfig[]} data
   * @category Records
   */
  /**
   * Applies a new dataset to the AssignmentStore and triggers calculations directly after. Use it to plug externally
   * fetched data into the store.
   *
   * ```javascript
   * await assignmentStore.loadDataAsync([{ eventId, resourceId }]);
   * // assignmentStore.first.event is available
   * ```
   *
   * @param {AssignmentModelConfig[]} data Array of AssignmentModel data objects
   * @function loadDataAsync
   * @category CRUD
   * @async
   */
  static get defaultConfig() {
    return {
      /**
       * CrudManager must load stores in the correct order. Lowest first.
       * @private
       */
      loadPriority: 300,
      /**
       * CrudManager must sync stores in the correct order. Lowest first.
       * @private
       */
      syncPriority: 300,
      storeId: 'assignments'
    };
  }
  add(newAssignments, ...args) {
    newAssignments = ArrayHelper.asArray(newAssignments);
    for (let i = 0; i < newAssignments.length; i++) {
      var _assignment$event;
      let assignment = newAssignments[i];
      if (!(assignment instanceof Model)) {
        newAssignments[i] = assignment = this.createRecord(assignment);
      }
      if (!this.isSyncingDataOnLoad && this.storage.findIndex('eventResourceKey', assignment.eventResourceKey, true) !== -1) {
        throw new Error(`Duplicate assignment Event: ${assignment.eventId} to resource: ${assignment.resourceId}`);
      }
      if ((_assignment$event = assignment.event) !== null && _assignment$event !== void 0 && _assignment$event.isCreating) {
        assignment.isCreating = true;
      }
    }
    return super.add(newAssignments, ...args);
  }
  includesAssignment(eventId, resourceId) {
    return this.storage.findIndex('eventResourceKey', `${eventId}-${resourceId}`, true) !== -1;
  }
  setStoreData(data) {
    if (this.usesSingleAssignment) {
      throw new Error('Data loading into AssignmentStore (multi-assignment mode) cannot be combined EventStore data containing resourceId (single-assignment mode)');
    }
    super.setStoreData(data);
  }
  //region Init & destroy
  // This index fixes poor performance when you add large number of events to an event store with large number of
  // events - if cache is missing existing records are iterated n² times.
  // https://github.com/bryntum/support/issues/3154#issuecomment-881336588
  set storage(storage) {
    super.storage = storage;
    // This allows a map based, fast lookup of assignments by their eventResourceKey.
    // This is so that the test for duplicate assignment adding is fast.
    this.storage.addIndex({
      property: 'eventResourceKey',
      dependentOn: {
        event: true,
        resource: true
      }
    });
  }
  get storage() {
    // Micro optimization to avoid expensive super call
    return this._storage || super.storage;
  }
  //endregion
  //region Stores
  // To not have to do instanceof checks
  get isAssignmentStore() {
    return true;
  }
  //endregion
  //region Recurrence
  /**
   * Returns a "fake" assignment used to identify a certain occurrence of a recurring event.
   * If passed the original event, it returns `originalAssignment`.
   * @param {Scheduler.model.AssignmentModel} originalAssignment
   * @param {Scheduler.model.EventModel} occurrence
   * @returns {Object} Temporary assignment
   * @internal
   */
  getOccurrence(originalAssignment, occurrence) {
    // Pass along the original assignment for non occurrence related calls
    if (!originalAssignment || !(occurrence !== null && occurrence !== void 0 && occurrence.isOccurrence)) {
      return originalAssignment;
    }
    // Not for saving chars, needed in fn below
    const me = this;
    return {
      id: `${occurrence.id}:a${originalAssignment.id}`,
      event: occurrence,
      resource: originalAssignment.resource,
      eventId: occurrence.id,
      resourceId: originalAssignment.resource.id,
      isAssignment: true,
      // This field is required to distinguish this fake assignment when event is being removed from UI
      isOccurrenceAssignment: true,
      // Not being an actual record, instanceMeta is stored on the store instead
      instanceMeta(instanceOrId) {
        return me.occurrenceInstanceMeta(this, instanceOrId);
      }
    };
  }
  // Per fake assignment instance meta, stored on store since fakes are always generated on demand
  occurrenceInstanceMeta(occurrenceAssignment, instanceOrId) {
    const me = this,
      instanceId = instanceOrId.id || instanceOrId,
      {
        id
      } = occurrenceAssignment;
    let {
      occurrenceMeta
    } = me;
    if (!occurrenceMeta) {
      occurrenceMeta = me.occurrenceMeta = {};
    }
    if (!occurrenceMeta[id]) {
      occurrenceMeta[id] = {};
    }
    return occurrenceMeta[id][instanceId] || (occurrenceMeta[id][instanceId] = {});
  }
  //endregion
  //region Mapping
  /**
   * Maps over event assignments.
   *
   * @param {Scheduler.model.EventModel} event
   * @param {Function} [fn]
   * @param {Function} [filterFn]
   * @returns {Scheduler.model.EventModel[]|Array}
   * @category Assignments
   */
  mapAssignmentsForEvent(event, fn, filterFn) {
    event = this.eventStore.getById(event);
    const fnSet = Boolean(fn),
      filterFnSet = Boolean(filterFn);
    if (fnSet || filterFnSet) {
      return event.assignments.reduce((result, assignment) => {
        const mapResult = fnSet ? fn(assignment) : assignment;
        if (!filterFnSet || filterFn(mapResult)) {
          result.push(mapResult);
        }
        return result;
      }, []);
    }
    return event.assignments;
  }
  /**
   * Maps over resource assignments.
   *
   * @param {Scheduler.model.ResourceModel|Number|String} resource
   * @param {Function} [fn]
   * @param {Function} [filterFn]
   * @returns {Scheduler.model.ResourceModel[]|Array}
   * @category Assignments
   */
  mapAssignmentsForResource(resource, fn, filterFn) {
    resource = this.resourceStore.getById(resource);
    const fnSet = Boolean(fn),
      filterFnSet = Boolean(filterFn);
    if (fnSet || filterFnSet) {
      return resource.assignments.reduce((result, assignment) => {
        const mapResult = fnSet ? fn(assignment) : assignment;
        if (!filterFnSet || filterFn(mapResult)) {
          result.push(mapResult);
        }
        return result;
      }, []);
    }
    return resource.assignments;
  }
  /**
   * Returns all assignments for a given event.
   *
   * @param {Scheduler.model.TimeSpan} event
   * @returns {Scheduler.model.AssignmentModel[]}
   * @category Assignments
   */
  getAssignmentsForEvent(event) {
    return event.assignments;
  }
  /**
   * Removes all assignments for given event
   *
   * @param {Scheduler.model.TimeSpan} event
   * @category Assignments
   */
  removeAssignmentsForEvent(event) {
    return this.remove(event.assignments);
  }
  /**
   * Returns all assignments for a given resource.
   *
   * @param {Scheduler.model.ResourceModel} resource
   * @returns {Scheduler.model.AssignmentModel[]}
   * @category Assignments
   */
  getAssignmentsForResource(resource) {
    resource = this.resourceStore.getById(resource);
    return resource.assignments;
  }
  /**
   * Removes all assignments for given resource
   *
   * @param {Scheduler.model.ResourceModel|*} resource
   * @category Assignments
   */
  removeAssignmentsForResource(resource) {
    this.remove(this.getAssignmentsForResource(resource));
  }
  /**
   * Returns all resources assigned to an event.
   *
   * @param {Scheduler.model.EventModel} event
   * @returns {Scheduler.model.ResourceModel[]}
   * @category Assignments
   */
  getResourcesForEvent(event) {
    return event.resources;
  }
  /**
   * Returns all events assigned to a resource
   *
   * @param {Scheduler.model.ResourceModel|String|Number} resource
   * @returns {Scheduler.model.TimeSpan[]}
   * @category Assignments
   */
  getEventsForResource(resource) {
    var _resource;
    resource = this.resourceStore.getById(resource);
    return (_resource = resource) === null || _resource === void 0 ? void 0 : _resource.events;
  }
  /**
   * Creates and adds assignment record(s) for a given event and resource(s).
   *
   * @param {Scheduler.model.TimeSpan} event
   * @param {Scheduler.model.ResourceModel|Scheduler.model.ResourceModel[]} resources The resource(s) to assign to the event
   * @param {Function} [assignmentSetupFn] A hook function which takes an assignment as its argument and must return an assignment.
   * @param {Boolean} [removeExistingAssignments] `true` to remove assignments for other resources
   * @returns {Scheduler.model.AssignmentModel[]} An array with the created assignment(s)
   * @category Assign
   */
  assignEventToResource(event, resources, assignmentSetupFn = null, removeExistingAssignments = false) {
    var _me$eventStore;
    const me = this,
      toRemove = removeExistingAssignments ? new Set(event.assignments) : null;
    resources = ArrayHelper.asArray(resources).map(r => r.$original ?? r);
    if ((_me$eventStore = me.eventStore) !== null && _me$eventStore !== void 0 && _me$eventStore.usesSingleAssignment) {
      var _event$assignments;
      // Use same code path as other single assignments if already assigned
      if ((_event$assignments = event.assignments) !== null && _event$assignments !== void 0 && _event$assignments.length) {
        if (!me.isEventAssignedToResource(event, resources[0])) {
          event.resource = resources[0];
        }
        return [];
      }
      // otherwise - set "resourceId" and proceed to assignment creation
      else {
        event.resourceId = resources[0].id;
      }
    }
    let newAssignments = [];
    me.suspendAutoCommit();
    // Assign
    resources.forEach(resource => {
      const existingAssignment = me.getAssignmentForEventAndResource(event, resource);
      if (!existingAssignment) {
        const assignment = {
          event,
          resource
        };
        newAssignments.push((assignmentSetupFn === null || assignmentSetupFn === void 0 ? void 0 : assignmentSetupFn(assignment)) ?? assignment);
      } else if (removeExistingAssignments) {
        toRemove.delete(existingAssignment);
      }
    });
    newAssignments = me.add(newAssignments);
    if (removeExistingAssignments) {
      me.remove(Array.from(toRemove));
    }
    // If true, will trigger a commit
    me.resumeAutoCommit();
    return newAssignments;
  }
  /**
   * Removes assignment record for a given event and resource.
   *
   * @param {Scheduler.model.TimeSpan|String|Number} event
   * @param {Scheduler.model.ResourceModel|String|Number} [resources] The resource to unassign the event from. If omitted, all resources of the events will be unassigned
   * @returns {Scheduler.model.AssignmentModel|Scheduler.model.AssignmentModel[]}
   * @category Assign
   */
  unassignEventFromResource(event, resources) {
    const me = this,
      assignmentsToRemove = [];
    if (!resources) {
      return me.removeAssignmentsForEvent(event);
    }
    resources = ArrayHelper.asArray(resources);
    for (let i = 0; i < resources.length; i++) {
      if (me.isEventAssignedToResource(event, resources[i])) {
        assignmentsToRemove.push(me.getAssignmentForEventAndResource(event, resources[i]));
      }
    }
    return me.remove(assignmentsToRemove);
  }
  /**
   * Checks whether an event is assigned to a resource.
   *
   * @param {Scheduler.model.EventModel|String|Number} event Event record or id
   * @param {Scheduler.model.ResourceModel|String|Number} resource Resource record or id
   * @returns {Boolean}
   * @category Assignments
   */
  isEventAssignedToResource(event, resource) {
    return Boolean(this.getAssignmentForEventAndResource(event, resource));
  }
  /**
   * Returns an assignment record for a given event and resource
   *
   * @param {Scheduler.model.EventModel|String|Number} event The event or its id
   * @param {Scheduler.model.ResourceModel|String|Number} resource The resource or its id
   * @returns {Scheduler.model.AssignmentModel}
   * @category Assignments
   */
  getAssignmentForEventAndResource(event, resource) {
    let assignments;
    // Note: In order to not evaluate conditions which do not have to be evaluated each condition is assigned to a
    // variable within the condition.
    if (!(event = this.eventStore.getById(event)) || !(assignments = event.assignments) ||
    // Also note that resources are looked for in the master store if chained, to handle dragging between
    // schedulers using chained versions of the same resource store. Needed since assignmentStore is shared and
    // might point to wrong resourceStore (can only point to one)
    !(resource = this.resourceStore.$master.getById(resource))) {
      return null;
    }
    return this.getOccurrence(assignments.find(a => {
      var _a$resource;
      return ((_a$resource = a.resource) === null || _a$resource === void 0 ? void 0 : _a$resource.$original) === resource.$original;
    }), event);
  }
  //endregion
});

const EngineMixin$7 = PartOfProject(CoreAssignmentStoreMixin.derive(AjaxStore));
/**
 * @module Scheduler/data/AssignmentStore
 */
/**
 * A store representing a collection of assignments between events in the {@link Scheduler.data.EventStore} and resources
 * in the {@link Scheduler.data.ResourceStore}.
 *
 * This store only accepts a model class inheriting from {@link Scheduler.model.AssignmentModel}.
 *
 * An AssignmentStore is usually connected to a project, which binds it to other related stores (EventStore,
 * ResourceStore and DependencyStore). The project also handles references (event, resource) to related records for the
 * records in the store.
 *
 * Resolving the references happens async, records are not guaranteed to have up to date references until calculations
 * are finished. To be certain that references are resolved, call `await project.commitAsync()` after store actions. Or
 * use one of the `xxAsync` functions, such as `loadDataAsync()`.
 *
 * Using `commitAsync()`:
 *
 * ```javascript
 * assignmentStore.data = [{ eventId, resourceId }, ...];
 *
 * // references (event, resource) not resolved yet
 *
 * await assignmentStore.project.commitAsync();
 *
 * // now they are
 * ```
 *
 * Using `loadDataAsync()`:
 *
 * ```javascript
 * await assignmentStore.loadDataAsync([{ eventId, resourceId }, ...]);
 *
 * // references (event, resource) are resolved
 * ```
 *
 * @mixes Scheduler/data/mixin/AssignmentStoreMixin
 * @mixes Scheduler/data/mixin/PartOfProject
 * @extends Core/data/AjaxStore
 */
class AssignmentStore extends AssignmentStoreMixin(EngineMixin$7) {
  static $name = 'AssignmentStore';
  static get defaultConfig() {
    return {
      modelClass: AssignmentModel
    };
  }
}
AssignmentStore._$name = 'AssignmentStore';

/**
 * @module Scheduler/data/mixin/ResourceStoreMixin
 */
/**
 * This is a mixin for the ResourceStore functionality. It is consumed by the {@link Scheduler.data.ResourceStore}.
 *
 * @mixin
 */
var ResourceStoreMixin = (Target => class ResourceStoreMixin extends (Target || Base) {
  static get $name() {
    return 'ResourceStoreMixin';
  }
  get isResourceStore() {
    return true;
  }
  /**
   * Add resources to the store.
   *
   * NOTE: References (events, assignments) on the resources are determined async by a calculation engine. Thus they
   * cannot be directly accessed after using this function.
   *
   * For example:
   *
   * ```javascript
   * const [resource] = resourceStore.add({ id });
   * // resource.events is not yet available
   * ```
   *
   * To guarantee references are set up, wait for calculations for finish:
   *
   * ```javascript
   * const [resource] = resourceStore.add({ id });
   * await resourceStore.project.commitAsync();
   * // resource.events is available (assuming EventStore is loaded and so on)
   * ```
   *
   * Alternatively use `addAsync()` instead:
   *
   * ```javascript
   * const [resource] = await resourceStore.addAsync({ id });
   * // resource.events is available (assuming EventStore is loaded and so on)
   * ```
   *
   * @param {Scheduler.model.ResourceModel|Scheduler.model.ResourceModel[]|ResourceModelConfig|ResourceModelConfig[]} records
   * Array of records/data or a single record/data to add to store
   * @param {Boolean} [silent] Specify `true` to suppress events
   * @returns {Scheduler.model.ResourceModel[]} Added records
   * @function add
   * @category CRUD
   */
  /**
   * Add resources to the store and triggers calculations directly after. Await this function to have up to date
   * references on the added resources.
   *
   * ```javascript
   * const [resource] = await resourceStore.addAsync({ id });
   * // resource.events is available (assuming EventStore is loaded and so on)
   * ```
   *
   * @param {Scheduler.model.ResourceModel|Scheduler.model.ResourceModel[]|ResourceModelConfig|ResourceModelConfig[]} records
   * Array of records/data or a single record/data to add to store
   * @param {Boolean} [silent] Specify `true` to suppress events
   * @returns {Scheduler.model.ResourceModel[]} Added records
   * @function addAsync
   * @category CRUD
   * @async
   */
  /**
   * Applies a new dataset to the ResourceStore. Use it to plug externally fetched data into the store.
   *
   * NOTE: References (events, assignments) on the resources are determined async by a calculation engine. Thus
   * they cannot be directly accessed after assigning the new dataset.
   *
   * For example:
   *
   * ```javascript
   * resourceStore.data = [{ id }];
   * // resourceStore.first.events is not yet available
   * ```
   *
   * To guarantee references are available, wait for calculations for finish:
   *
   * ```javascript
   * resourceStore.data = [{ id }];
   * await resourceStore.project.commitAsync();
   * // resourceStore.first.events is available
   * ```
   *
   * Alternatively use `loadDataAsync()` instead:
   *
   * ```javascript
   * await resourceStore.loadDataAsync([{ id }]);
   * // resourceStore.first.events is available
   * ```
   *
   * @member {ResourceModelConfig[]} data
   * @category Records
   */
  /**
   * Applies a new dataset to the ResourceStore and triggers calculations directly after. Use it to plug externally
   * fetched data into the store.
   *
   * ```javascript
   * await resourceStore.loadDataAsync([{ id }]);
   * // resourceStore.first.events is available
   * ```
   *
   * @param {ResourceModelConfig[]} data Array of ResourceModel data objects
   * @function loadDataAsync
   * @category CRUD
   * @async
   */
  static get defaultConfig() {
    return {
      /**
       * CrudManager must load stores in the correct order. Lowest first.
       * @private
       */
      loadPriority: 200,
      /**
       * CrudManager must sync stores in the correct order. Lowest first.
       * @private
       */
      syncPriority: 100,
      storeId: 'resources',
      autoTree: true
    };
  }
  construct(config) {
    super.construct(config);
    if (!this.modelClass.isResourceModel) {
      throw new Error('Model for ResourceStore must subclass ResourceModel');
    }
  }
  removeAll() {
    const result = super.removeAll(...arguments);
    // Removing all resources removes all assignments
    result && this.assignmentStore.removeAll();
    return result;
  }
  // Apply id changes also to assignments (used to be handled automatically by relations earlier, but engine does not
  // care about ids so needed now)
  // problems:
  // 1. orientation/HorizontalRendering listens to assignment store changes and is trying to refresh view
  // When we update resource id on assignment, listener will be invoked and view will try to refresh. And it will
  // fail, because row is not updated yet. Flag is raised on resource store to make HorizontalRendering to skip
  // refreshing view in this particular case of resource id changing
  onRecordIdChange({
    record,
    oldValue,
    value
  }) {
    super.onRecordIdChange({
      record,
      oldValue,
      value
    });
    if (record.isFieldModified('id')) {
      this.isChangingId = true;
      record.updateAssignmentResourceIds();
      this.isChangingId = false;
    }
  }
  // Cache used by VerticalRendering, reset from there
  get allResourceRecords() {
    return this._allResourceRecords || (this._allResourceRecords = this.getAllDataRecords());
  }
  /**
   * Returns all resources that have no events assigned during the specified time range.
   * @param {Date} startDate Time range start date
   * @param {Date} endDate Time range end date
   * @returns {Scheduler.model.ResourceModel[]} Resources without events
   */
  getAvailableResources({
    startDate,
    endDate
  }) {
    return this.query(resource => this.eventStore.isDateRangeAvailable(startDate, endDate, null, resource));
  }
});

/**
 * @module Scheduler/model/mixin/ResourceModelMixin
 */
/**
 * Mixin that holds configuration shared between resources in Scheduler and Scheduler Pro.
 * @mixin
 */
var ResourceModelMixin = (Target => class ResourceModelMixin extends Target {
  static get $name() {
    return 'ResourceModelMixin';
  }
  // Flag checked by ResourceStore to make sure it uses a valid subclass
  static get isResourceModel() {
    return true;
  }
  /**
   * Set value for the specified field(s), triggering engine calculations immediately. See
   * {@link Core.data.Model#function-set Model#set()} for arguments.
   *
   * This does not matter much on the resource itself, but is of importance when manipulating its references:
   *
   * ```javascript
   * assignment.set('resourceId', 2);
   * // resource.assignments is not yet up to date
   *
   * await assignment.setAsync('resourceId', 2);
   * // resource.assignments is up to date
   * ```
   *
   * @param {String|Object} field The field to set value for, or an object with multiple values to set in one call
   * @param {*} value Value to set
   * @param {Boolean} [silent=false] Set to true to not trigger events
   * automatically.
   * @function setAsync
   * @category Editing
   * @async
   */
  //region Fields
  static get fields() {
    return [
    /**
     * Unique identifier
     * @field {String|Number} id
     * @category Common
     */
    /**
     * Get or set resource name
     * @field {String} name
     * @category Common
     */
    {
      name: 'name',
      type: 'string',
      persist: true
    },
    /**
     * Controls the primary color used for events assigned to this resource. Can be overridden per event using
     * EventModels {@link Scheduler/model/mixin/EventModelMixin#field-eventColor eventColor config}. See Schedulers
     * {@link Scheduler.view.mixin.TimelineEventRendering#config-eventColor eventColor config} for available
     * colors.
     * @field {String} eventColor
     * @category Styling
     */
    'eventColor',
    /**
     * Controls the style used for events assigned to this resource. Can be overridden per event using
     * EventModels {@link Scheduler/model/mixin/EventModelMixin#field-eventStyle eventStyle config}. See Schedulers
     * {@link Scheduler.view.mixin.TimelineEventRendering#config-eventStyle eventStyle config} for available
     * options.
     * @field {String} eventStyle
     * @category Styling
     */
    'eventStyle',
    /**
     * Fully qualified image URL, used by `ResourceInfoColumn` and vertical modes `ResourceHeader` to display a miniature image
     * for the resource.
     * @field {String} imageUrl
     * @category Styling
     */
    'imageUrl',
    /**
     * Image name relative to {@link Scheduler/view/mixin/SchedulerEventRendering#config-resourceImagePath},
     * used by `ResourceInfoColumn` and vertical modes `ResourceHeader` to display a miniature image
     * for the resource.
     * @field {String} image
     * @category Styling
     */
    'image',
    /**
     * Margin from rows edge to topmost event bar for this resource, in px.
     * @field {Number} resourceMargin
     * @category Layout
     */
    {
      name: 'resourceMargin',
      type: 'number'
    },
    /**
     * Margin between stacked event bars for this resource, in px.
     * @field {Number} barMargin
     * @category Layout
     */
    {
      name: 'barMargin',
      type: 'number'
    },
    /**
     * Base height of this resource, in px. When unset, Schedulers configured rowHeight is used.
     *
     * This value is used in horizontal mode to determine row height. When stacking, it is used as input for
     * calculating the actual row height:
     *
     * ```javascript
     * row.height = (resource.rowHeight - resourceMargin * 2) * overlap count - barMargin * (overlap count - 1)
     * ```
     *
     * When packing or overlapping, it is used as the actual row height.
     *
     * @field {Number} rowHeight
     * @category Layout
     */
    /**
     * Base width of this resource, in px. If not set, the `columnWidth` specified in
     * the Scheduler's configured {@link Scheduler.view.Scheduler#config-resourceColumns} is used.
     *
     * This value is used in vertical mode to determine column width.
     *
     * @field {Number} columnWidth
     * @category Layout
     */
    /**
     * Specify this to use a resource specific event layout in horizontal mode, see
     * {@link Scheduler.view.mixin.SchedulerEventRendering#config-eventLayout} for options.
     *
     * When unset (the default) Schedulers setting is used.
     *
     * @field {'stack'|'pack'|'mixed'|'none'} eventLayout
     * @category Layout
     */
    'eventLayout'];
  }
  //endregion
  //region Id change
  updateAssignmentResourceIds() {
    this.assigned.forEach(assignment => {
      assignment.resourceId = this.id;
    });
  }
  syncId(value) {
    super.syncId(value);
    this.updateAssignmentResourceIds();
  }
  //endregion
  //region Getters
  // Documented in Scheduler.model.ResourceModel, SchedulerPro.model.ResourceModel, Gantt.model.ResourceModel
  get events() {
    return this.assignments.reduce((events, assignment) => {
      if (assignment.event) {
        events.push(assignment.event);
      }
      return events;
    }, []);
  }
  /**
   * Returns all assignments for the resource
   *
   * @property {Scheduler.model.AssignmentModel[]}
   * @category Common
   */
  get assignments() {
    return this.assigned ? [...this.assigned] : [];
  }
  set assignments(assignments) {
    // Engine does not allow assigning to `assigned`, handle it here
    assignments.forEach(assignment => {
      assignment.resource = this;
    });
  }
  /**
   * Returns an array of events, associated with this resource
   *
   * @deprecated 5.3.6 Use the events property instead
   *
   * @returns {Scheduler.model.EventModel[]}
   */
  getEvents() {
    VersionHelper.deprecate('scheduler', '6.0.0', 'getEvents() is deprecated, use the events property instead');
    return this.events;
  }
  /**
   * Returns `true` if the resource can be persisted.
   * In a flat store, a resource is always considered persistable. In a tree store, a resource is considered
   * persistable if its parent node is persistable.
   *
   * @property {Boolean}
   * @readonly
   * @category Editing
   */
  get isPersistable() {
    return super.isPersistable && (!this.parent || this.parent.isPersistable);
  }
  //endregion
  /**
   * Unassigns this Resource from all its Events
   */
  unassignAll() {
    this.assignments && this.assignmentStore.remove(this.assignments);
  }
  /**
   * Returns the initials (first letter of the first & last space-separated word in the name) or an empty string
   * if this resource has no name. You can override this method in a ResourceModel subclass to provide your own implementation
   *
   * @property {String}
   * @readonly
   * @category Common
   */
  get initials() {
    const {
      name = ''
    } = this;
    if (!name) {
      return '';
    }
    const names = name.split(' '),
      firstInitial = names[0][0],
      lastInitial = names.length > 1 ? names[names.length - 1][0] : '';
    return firstInitial + lastInitial;
  }
  isWorkingTime(date) {
    var _this$project, _calendar$isWorkingTi;
    const calendar = this.effectiveCalendar || ((_this$project = this.project) === null || _this$project === void 0 ? void 0 : _this$project.calendar);
    return !calendar || ((_calendar$isWorkingTi = calendar.isWorkingTime) === null || _calendar$isWorkingTi === void 0 ? void 0 : _calendar$isWorkingTi.call(calendar, date));
  }
});

const EngineMixin$6 = CoreResourceMixin;
/**
 * @module Scheduler/model/ResourceModel
 */
/**
 * This class represent a single Resource in scheduler, usually added to a {@link Scheduler.data.ResourceStore}.
 *
 * It is a subclass of  {@link Core.data.Model}. Please refer to the documentation for that class to become familiar
 * with the base interface of the resource.
 *
 * ## Fields and references
 *
 * A resource has a few predefined fields, see Fields below. If you want to add more fields with meta data describing
 * your resources then you should subclass this class:
 *
 * ```javascript
 * class MyResource extends ResourceModel {
 *   static get fields() {
 *     return [
 *       // "id" and "name" fields are already provided by the superclass
 *       { name: 'company', type : 'string' }
 *     ];
 *   }
 * });
 * ```
 *
 * If you want to use other names in your data for the id and name fields you can configure them as seen below:
 *
 * ```javascript
 * class MyResource extends ResourceModel {
 *   static get fields() {
 *     return [
 *        { name: 'name', dataSource: 'userName' }
 *     ];
 *   },
 * });
 * ```
 *
 * After load and project normalization, these references are accessible (assuming their respective stores are loaded):
 * - `{@link #property-assignments}` - The linked assignment records
 * - `{@link #property-events}` - The linked (through assignments) event records
 *
 * ## Async resolving of references
 *
 * As described above, a resource has links to assignments and events. These references are populated async, using the
 * calculation engine of the project that the resource via its store is a part of. Because of this asyncness, references
 * cannot be used immediately after assignment modifications:
 *
 * ```javascript
 * assignment.resourceId = 2;
 * // resource.assignments is not yet up to date
 * ```
 *
 * To make sure references are updated, wait for calculations to finish:
 *
 * ```javascript
 * assignment.resourceId = 2;
 * await assignment.project.commitAsync();
 * // resource.assignments is up to date
 * ```
 *
 * As an alternative, you can also use `setAsync()` to trigger calculations directly after the change:
 *
 * ```javascript
 * await assignment.setAsync({ resourceId : 2});
 * // resource.assignments is up to date
 * ```
 *
 * @extends Grid/data/GridRowModel
 * @mixes Scheduler/model/mixin/ResourceModelMixin
 */
class ResourceModel extends ResourceModelMixin(PartOfProject(EngineMixin$6.derive(GridRowModel))) {
  /**
   * Get associated events
   *
   * @member {Scheduler.model.EventModel[]} events
   * @readonly
   * @category Common
   */
  static $name = 'ResourceModel';
}
ResourceModel.exposeProperties();
ResourceModel._$name = 'ResourceModel';

const EngineMixin$5 = PartOfProject(CoreResourceStoreMixin.derive(AjaxStore));
/**
 * @module Scheduler/data/ResourceStore
 */
/**
 * A store holding all the {@link Scheduler.model.ResourceModel resources} to be rendered into a
 * {@link Scheduler.view.Scheduler Scheduler}.
 *
 * This store only accepts a model class inheriting from {@link Scheduler.model.ResourceModel}.
 *
 * A ResourceStore is usually connected to a project, which binds it to other related stores (EventStore,
 * AssignmentStore and DependencyStore). The project also handles references (assignments, events) to related records
 * for the records in the store.
 *
 * Resolving the references happens async, records are not guaranteed to have up to date references until calculations
 * are finished. To be certain that references are resolved, call `await project.commitAsync()` after store actions. Or
 * use one of the `xxAsync` functions, such as `loadDataAsync()`.
 *
 * Using `commitAsync()`:
 *
 * ```javascript
 * resourceStore.data = [{ id }, ...];
 *
 * // references (assignments, events) not resolved yet
 *
 * await resourceStore.project.commitAsync();
 *
 * // now they are
 * ```
 *
 * Using `loadDataAsync()`:
 *
 * ```javascript
 * await resourceStore.loadDataAsync([{ id }, ...]);
 *
 * // references (assignments, events) are resolved
 * ```
 *
 * @mixes Scheduler/data/mixin/PartOfProject
 * @mixes Scheduler/data/mixin/ResourceStoreMixin
 * @extends Core/data/AjaxStore
 */
class ResourceStore extends ResourceStoreMixin(EngineMixin$5) {
  static get defaultConfig() {
    return {
      modelClass: ResourceModel
    };
  }
}
ResourceStore._$name = 'ResourceStore';

/**
 * @module Scheduler/data/mixin/EventStoreMixin
 */
/**
 * This is a mixin, containing functionality related to managing events.
 *
 * It is consumed by the regular {@link Scheduler.data.EventStore} class and the Scheduler Pro's `EventStore` class.
 *
 * @mixin
 */
var EventStoreMixin = (Target => class EventStoreMixin extends (Target || Base) {
  static get $name() {
    return 'EventStoreMixin';
  }
  //region Init & destroy
  construct(config) {
    super.construct(config);
    this.autoTree = true;
  }
  //endregion
  //region Events records, iteration etc.
  set filtersFunction(filtersFunction) {
    super.filtersFunction = filtersFunction;
  }
  get filtersFunction() {
    // Generate the real filterFn.
    const result = super.filtersFunction;
    // We always filter *in* records which are being created by the UI.
    if (result && result !== FunctionHelper.returnTrue) {
      return r => r.isCreating || result(r);
    }
    return result;
  }
  /**
   * Returns a `Map`, keyed by `YYYY-MM-DD` date keys containing event counts for all the days
   * between the passed `startDate` and `endDate`. Occurrences of recurring events are included.
   *
   * Example:
   *
   * ```javascript
   *  eventCounts = eventStore.getEventCounts({
   *      startDate : scheduler.timeAxis.startDate,
   *      endDate   : scheduler.timeAxis.endDate
   *  });
   * ```
   *
   * @param {Object} options An options object determining which events to return
   * @param {Date} options.startDate The start date for the range of events to include.
   * @param {Date} [options.endDate] The end date for the range of events to include.
   * @category Events
   */
  getEventCounts(options) {
    const me = this,
      {
        filtersFunction,
        added
      } = me,
      // Must use getEvents so that the loadDateRange event is triggered.
      result = me.getEvents({
        ...options,
        storeFilterFn: me.isFiltered ? me.reapplyFilterOnAdd ? filtersFunction : eventRecord => added.includes(eventRecord) ? me.indexOf(eventRecord) > -1 : filtersFunction(eventRecord) : null,
        dateMap: options.dateMap || true
      });
    result.forEach((value, key) => result.set(key, value.length));
    return result;
  }
  /**
   * Calls the supplied iterator function once for every scheduled event, providing these arguments
   * - event : the event record
   * - startDate : the event start date
   * - endDate : the event end date
   *
   * Returning false cancels the iteration.
   *
   * @param {Function} fn iterator function
   * @param {Object} [thisObj] `this` reference for the function
   * @category Events
   */
  forEachScheduledEvent(fn, thisObj = this) {
    this.forEach(event => {
      const {
        startDate,
        endDate
      } = event;
      if (startDate && endDate) {
        return fn.call(thisObj, event, startDate, endDate);
      }
    });
  }
  /**
   * Returns an object defining the earliest start date and the latest end date of all the events in the store.
   *
   * @returns {Object} An object with 'startDate' and 'endDate' properties (or null values if data is missing).
   * @category Events
   */
  getTotalTimeSpan() {
    let earliest = new Date(9999, 0, 1),
      latest = new Date(0);
    this.forEach(event => {
      if (event.startDate) {
        earliest = DateHelper.min(event.startDate, earliest);
      }
      if (event.endDate) {
        latest = DateHelper.max(event.endDate, latest);
      }
    });
    earliest = earliest < new Date(9999, 0, 1) ? earliest : null;
    latest = latest > new Date(0) ? latest : null;
    // keep last calculated value to be able to track total timespan changes
    return this.lastTotalTimeSpan = {
      startDate: earliest || null,
      endDate: latest || earliest || null
    };
  }
  /**
   * Checks if given event record is persistable. By default it always is, override EventModels `isPersistable` if you
   * need custom logic.
   *
   * @param {Scheduler.model.EventModel} event
   * @returns {Boolean}
   * @category Events
   */
  isEventPersistable(event) {
    return event.isPersistable;
  }
  //endregion
  //region Resource
  /**
   * Checks if a date range is allocated or not for a given resource.
   * @param {Date} start The start date
   * @param {Date} end The end date
   * @param {Scheduler.model.EventModel|null} excludeEvent An event to exclude from the check (or null)
   * @param {Scheduler.model.ResourceModel} resource The resource
   * @returns {Boolean} True if the timespan is available for the resource
   * @category Resource
   */
  isDateRangeAvailable(start, end, excludeEvent, resource) {
    // NOTE: Also exists in TaskStore.js
    // Cannot assign anything to generated parents
    if (resource.data.generatedParent) {
      return false;
    }
    // This should be a collection of unique event records
    const allEvents = new Set(this.getEventsForResource(resource));
    // In private mode we can pass an AssignmentModel. In this case, we assume that multi-assignment is used.
    // So we need to make sure that other resources are available for this time too.
    // No matter if the event retrieved from the assignment belongs to the target resource or not.
    // We gather all events from the resources the event is assigned to except of the one from the assignment record.
    // Note, events from the target resource are added above.
    if (excludeEvent !== null && excludeEvent !== void 0 && excludeEvent.isAssignment) {
      const currentEvent = excludeEvent.event,
        resources = currentEvent.resources;
      resources.forEach(resource => {
        // Ignore events for the resource which is passed as an AssignmentModel to excludeEvent
        if (resource.id !== excludeEvent.resourceId) {
          this.getEventsForResource(resource).forEach(event => allEvents.add(event));
        }
      });
    }
    if (excludeEvent) {
      const eventToRemove = excludeEvent.isAssignment ? excludeEvent.event : excludeEvent;
      allEvents.delete(eventToRemove);
    }
    return !Array.from(allEvents).some(event => event.isScheduled && DateHelper.intersectSpans(start, end, event.startDate, event.endDate));
  }
  /**
   * Filters the events associated with a resource, based on the function provided. An array will be returned for those
   * events where the passed function returns true.
   * @param {Scheduler.model.ResourceModel} resource
   * @param {Function} fn The function
   * @param {Object} [thisObj] `this` reference for the function
   * @returns {Scheduler.model.EventModel[]} the events in the time span
   * @private
   * @category Resource
   */
  filterEventsForResource(resource, fn, thisObj = this) {
    return resource.getEvents(this).filter(fn.bind(thisObj));
  }
  /**
   * Returns all resources assigned to an event.
   *
   * @param {Scheduler.model.EventModel|String|Number} event
   * @returns {Scheduler.model.ResourceModel[]}
   * @category Resource
   */
  getResourcesForEvent(event) {
    // If we are sent an occurrence, use its parent
    if (event.isOccurrence) {
      event = event.recurringTimeSpan;
    }
    return this.assignmentStore.getResourcesForEvent(event);
  }
  /**
   * Returns all events assigned to a resource.
   * *NOTE:* this does not include occurrences of recurring events. Use the
   * {@link Scheduler/data/mixin/GetEventsMixin#function-getEvents} API to include occurrences of recurring events.
   * @param {Scheduler.model.ResourceModel|String|Number} resource Resource or resource id.
   * @returns {Scheduler.model.EventModel[]}
   * @category Resource
   */
  getEventsForResource(resource) {
    return this.assignmentStore.getEventsForResource(resource);
  }
  //endregion
  //region Assignment
  /**
   * Returns all assignments for a given event.
   *
   * @param {Scheduler.model.EventModel|String|Number} event
   * @returns {Scheduler.model.AssignmentModel[]}
   * @category Assignment
   */
  getAssignmentsForEvent(event) {
    return this.assignmentStore.getAssignmentsForEvent(event) || [];
  }
  /**
   * Returns all assignments for a given resource.
   *
   * @param {Scheduler.model.ResourceModel|String|Number} resource
   * @returns {Scheduler.model.AssignmentModel[]}
   * @category Assignment
   */
  getAssignmentsForResource(resource) {
    return this.assignmentStore.getAssignmentsForResource(resource) || [];
  }
  /**
   * Creates and adds assignment record for a given event and a resource.
   *
   * @param {Scheduler.model.EventModel|String|Number} event
   * @param {Scheduler.model.ResourceModel|String|Number|Scheduler.model.ResourceModel[]|String[]|Number[]} resource The resource(s) to assign to the event
   * @param {Boolean} [removeExistingAssignments] `true` to first remove existing assignments
   * @returns {Scheduler.model.AssignmentModel[]} An array with the created assignment(s)
   * @category Assignment
   */
  assignEventToResource(event, resource, removeExistingAssignments = false) {
    return this.assignmentStore.assignEventToResource(event, resource, undefined, removeExistingAssignments);
  }
  /**
   * Removes assignment record for a given event and a resource.
   *
   * @param {Scheduler.model.EventModel|String|Number} event
   * @param {Scheduler.model.ResourceModel|String|Number} resource
   * @category Assignment
   */
  unassignEventFromResource(event, resource) {
    this.assignmentStore.unassignEventFromResource(event, resource);
  }
  /**
   * Reassigns an event from an old resource to a new resource
   *
   * @param {Scheduler.model.EventModel}    event    An event or id of the event to reassign
   * @param {Scheduler.model.ResourceModel|Scheduler.model.ResourceModel[]} oldResource A resource or id to unassign from
   * @param {Scheduler.model.ResourceModel|Scheduler.model.ResourceModel[]} newResource A resource or id to assign to
   * @category Assignment
   */
  reassignEventFromResourceToResource(event, oldResource, newResource) {
    const me = this,
      newResourceId = Model.asId(newResource),
      assignment = me.assignmentStore.getAssignmentForEventAndResource(event, oldResource);
    if (assignment) {
      assignment.resourceId = newResourceId;
    } else {
      me.assignmentStore.assignEventToResource(event, newResource);
    }
  }
  /**
   * Checks whether an event is assigned to a resource.
   *
   * @param {Scheduler.model.EventModel|String|Number} event
   * @param {Scheduler.model.ResourceModel|String|Number} resource
   * @returns {Boolean}
   * @category Assignment
   */
  isEventAssignedToResource(event, resource) {
    return this.assignmentStore.isEventAssignedToResource(event, resource);
  }
  /**
   * Removes all assignments for given event
   *
   * @param {Scheduler.model.EventModel|String|Number} event
   * @category Assignment
   */
  removeAssignmentsForEvent(event) {
    this.assignmentStore.removeAssignmentsForEvent(event);
  }
  /**
   * Removes all assignments for given resource
   *
   * @param {Scheduler.model.ResourceModel|String|Number} resource
   * @category Assignment
   */
  removeAssignmentsForResource(resource) {
    this.assignmentStore.removeAssignmentsForResource(resource);
  }
  //endregion
});

/**
 * @module Scheduler/data/mixin/GetEventsMixin
 */
const returnTrue = () => true,
  notRecurring = event => !event.isRecurring;
/**
 * Mixing containing functionality for retrieving a range of events, mainly used during rendering.
 *
 * Consumed by EventStore in Calendar, Scheduler & Scheduler Pro and TaskStore in Gantt.
 *
 * @mixin
 */
var GetEventsMixin = (Target => class GetEventsMixin extends Target {
  static $name = 'GetEventsMixin';
  /**
   * Returns an array of events for the date range specified by the `startDate` and `endDate` options.
   *
   * By default, for any date, this includes any event which *intersects* that date.
   *
   * To only include events that are fully contained *within* the date range, pass the `allowPartial`
   * option as `false`.
   *
   * By default, any occurrences of recurring events are included in the resulting array (not applicable in Gantt). If
   * that is not required, pass the `includeOccurrences` option as `false`. **Note that if `includeOccurrences` is
   * `true`, the start date and end date options are mandatory. The method must know what range of occurrences needs
   * to be generated and returned.**
   *
   * Example:
   *
   * ```javascript
   *  visibleEvents = eventStore.getEvents({
   *      resourceRecord : myResource,
   *      startDate      : scheduler.timeAxis.startDate,
   *      endDate        : scheduler.timeAxis.endDate
   *  });
   * ```
   *
   * @param {Object} options An options object determining which events to return
   * @param {Date} [options.date] If only one date is required, pass this option instead of the
   * `startDate` and `endDate` options.
   * @param {Date} options.startDate The start date for the range of events to include.
   * @param {Date} [options.endDate] The end date for the range of events to include.
   * @param {Scheduler.model.ResourceModel} [options.resourceRecord] Pass a resource to only return events assigned to
   *   this resource. Not supported when using the `dateMap` option (see below)
   * @param {Function} [options.filter] A function to filter out events which are not required.
   * @param {Boolean} [options.ignoreFilters] By default, store filters are honoured. Pass this
   * as `true` to include filtered out events.
   * @param {Boolean} [options.includeOccurrences=true] Occurrences of recurring events are included by default.
   * @param {Boolean} [options.allowPartial=true] Events which start before or after the range, but *intersect* the
   *   range are included by default.
   * @param {Boolean} [options.startOnly] Pass `true` to only include events which *start on* each date in the range.
   * @param {Boolean} [options.onlyAssigned] Pass `true` to only include events that are assigned to a resource
   * @param {Boolean|Map} [options.dateMap] Populates the passed `Map`, or if passed as `true`, creates and
   * returns a new `Map`. The keys are `YYYY-MM-DD` date strings and the entries are arrays of
   * {@link Scheduler.model.EventModel EventModel}s.
   * @returns {Scheduler.model.EventModel[]|Map} Events which match the passed configuration.
   * @category Events
   */
  getEvents({
    filter,
    date,
    startDate,
    // Events which intersect the startDate/endDate
    endDate,
    // will be returned
    startOnly,
    // Only events which start on each date will be returned
    includeOccurrences,
    // Interpolate occurrences into the returned event set
    allowPartial,
    // Include events which *intersect* the date range
    onlyAssigned = false,
    // Only include events that are assigned to a resource
    dateMap = false,
    // Return a Map keyed by date each value being an array of events
    dayTime = null,
    // Private option. Select which date index to look up events in depending on the date
    // we are examining in the date iteration process. Some callers may want to use
    // different indices depending on the stage through the date iteration.
    // See Calendar package for usage.
    getDateIndex
  }) {
    const me = this,
      options = arguments[0],
      {
        lastDateRange,
        added,
        filtersFunction
      } = me,
      passedFilter = filter;
    // Add filtering for only assigned events if requested.
    if (onlyAssigned) {
      options.filter = passedFilter ? e => passedFilter(e) && e.resources.length : e => e.resources.length;
    }
    // Note that we cannot use defaulting in the argument block because we pass
    // the incoming options object down into implementations.
    if (!('startDate' in options)) {
      startDate = options.startDate = date;
    }
    if (!('includeOccurrences' in options)) {
      includeOccurrences = options.includeOccurrences = true;
    }
    if (!('allowPartial' in options)) {
      allowPartial = options.allowPartial = !startOnly;
    }
    // We can't use me.filtersFunction if reapplyFilterOnAdd is false because there may be newly
    // added events which may not be subject to the filter. Records which are still in
    // the added bag must be tested for presence using indexOf so as to be always in sync
    // with the store being refiltered. Parens help readability.
    // Don't use the store's filtering function if we were asked to ignore filters.
    // eslint-disable-next-line no-extra-parens
    options.storeFilterFn = me.isFiltered && !options.ignoreFilters ? me.reapplyFilterOnAdd ? filtersFunction : eventRecord => added.includes(eventRecord) ? me.indexOf(eventRecord) > -1 : filtersFunction(eventRecord) : null;
    // Default to a one day range if only startDate passed
    if (!endDate) {
      if (startDate) {
        endDate = options.endDate = DateHelper.clearTime(startDate);
        endDate.setDate(endDate.getDate() + 1);
      }
      // If no dates passed, the dateFilter will include all.
      else {
        // We need to know what occurrences to generate.
        if (includeOccurrences) {
          throw new Error('getEvents MUST be passed startDate and endDate if recurring occurrences are requested');
        }
        options.dateFilter = returnTrue;
      }
    }
    if (!options.dateFilter) {
      // Must start in the date range
      if (startOnly) {
        options.dateFilter = e => {
          // Avoid hitting getter twice. Use batched value if present.
          const eventStartDate = e.hasBatchedChange('startDate') ? e.get('startDate') : e.startDate;
          return eventStartDate && !(DateHelper.clearTime(eventStartDate) - startDate);
        };
      }
      // Any intersection with our date range
      else if (allowPartial) {
        options.dateFilter = e => {
          // Avoid hitting getter twice. Use batched value if present.
          const eventStartDate = e.hasBatchedChange('startDate') ? e.get('startDate') : e.startDate,
            eventEndDate = e.hasBatchedChange('endDate') ? e.get('endDate') : e.endDate || eventStartDate,
            isMilestone = !(eventStartDate - eventEndDate);
          return eventStartDate && (isMilestone ? DateHelper.betweenLesserEqual(eventStartDate, startDate, endDate) : DateHelper.intersectSpans(eventStartDate, eventEndDate, startDate, endDate));
        };
      }
      // Must be wholly contained with the our range
      else {
        options.dateFilter = e => {
          // Avoid hitting getter twice. Use batched value if present.
          const eventStartDate = e.hasBatchedChange('startDate') ? e.get('startDate') : e.startDate,
            eventEndDate = e.hasBatchedChange('endDate') ? e.get('endDate') : e.endDate || eventStartDate;
          return eventStartDate && eventStartDate >= startDate && eventEndDate <= endDate;
        };
      }
    }
    const newDateRange = {
      startDate,
      endDate
    };
    // Ensure the listeners are present
    me.processConfiguredListeners();
    /**
     * Fired when a range of events is requested from the {@link #function-getEvents} method.
     *
     * <div class="note">
     * This event fires <span style="font-weight:bold">every time</span> a range of events is
     * requested from the store.
     * </div>
     *
     * An application may have one of two levels of interest in events being read from a store.<br>
     *
     * 1.  To be notified when <span style="font-weight:bold">any</span> event block is requested regardless of what the
     * date range is.
     * 2.  To be notified when a <span style="font-weight:bold">new date range</span> is requested.
     *
     * This event allows both types of application to be written. The `changed` property is
     * set if a different date range is requested.
     *
     * ```javascript
     * new Scheduler({
     *     eventStore : {
     *         listeners : {
     *             loadDateRange({ new : { startDate, endDate }, changed }) {
     *                 // Load new data if user is requesting a different time window.
     *                 if (changed) {
     *                     fetch(...);
     *                 }
     *             }
     *         }
     *     },
     *     ...
     * });
     * ```
     *
     * @event loadDateRange
     * @param {Scheduler.data.EventStore} source This EventStore
     * @param {Object} old The old date range
     * @param {Date} old.startDate the old start date.
     * @param {Date} old.endDate the old end date.
     * @param {Object} new The new date range
     * @param {Date} new.startDate the new start date.
     * @param {Date} new.endDate the new end date.
     * @param {Boolean} changed `true` if the date range is different from the last time a request was made.
     */
    me.trigger('loadDateRange', {
      old: lastDateRange || {},
      new: Objects.clone(newDateRange),
      changed: Boolean(!lastDateRange || lastDateRange.startDate - newDateRange.startDate || lastDateRange.endDate - newDateRange.endDate)
    });
    // Dates are mutable, so we must keep our own copy.
    me.lastDateRange = Objects.clone(newDateRange);
    return dateMap ? me.getEventsAsMap(options) : me.getEventsAsArray(options);
  }
  /**
   * Internal implementation for {@link #function-getEvents} to use when not using dateMap.
   * @private
   */
  getEventsAsArray({
    filter,
    date,
    resourceRecord,
    startDate = date,
    // Events which intersect the startDate/endDate
    endDate,
    // will be returned
    startOnly,
    // Only events which start on each date will be returned
    includeOccurrences = true,
    // Interpolate occurrences into the returned event set
    dayTime = null,
    // Injected by the getEvents master method
    dateFilter,
    storeFilterFn,
    // Private option. Select which date index to look up events in depending on the date
    // we are examining in the date iteration process. Some callers may want to use
    // different indices depending on the stage through the date iteration.
    // See Calendar package for usage.
    getDateIndex
  }) {
    const me = this,
      events = [],
      count = storeFilterFn ? me.count : me.allCount;
    if (count) {
      let candidateEvents = resourceRecord ? me.getEventsForResource(resourceRecord) : null;
      // If there *was* a resourceRecord, candidateEvents will already be set up using me.getEventsForResource.
      // If no resourceRecord specified, we are gathering by date, so use the indices.
      if (!resourceRecord) {
        const dateIndex = me.useDayIndex(dayTime),
          eventSet = new Set(),
          indexName = startOnly ? 'startDate' : 'date';
        // Add all recurring events which started on or before our date range.
        me.recurringEvents.forEach(e => {
          if (dateIndex.dayTime.startOfDay(e.startDate) <= startDate) {
            eventSet.add(e);
          }
        });
        // Iterate the date range, using the indices to find qualified events.
        for (const date = new Date(startDate); date < endDate; date.setDate(date.getDate() + 1)) {
          const coincidingEvents = dateIndex.get(getDateIndex ? getDateIndex(date) : indexName, date);
          coincidingEvents === null || coincidingEvents === void 0 ? void 0 : coincidingEvents.forEach(e => eventSet.add(e));
        }
        // We gathered all events which *coincide* with each date.
        // We also added in all recurring events which started on or before our date range.
        // All these were made unique by the Set.
        // Return it to array form.
        candidateEvents = [...eventSet];
      }
      // Events found from the date indices won't be filtered.
      // On the other side, when using getEventForResource we will get all events for
      // the resource even if the EventStore is filtered, handle this by excluding "invisible" events here
      if (storeFilterFn) {
        candidateEvents = candidateEvents.filter(storeFilterFn);
      }
      // Go through candidates.
      // For a recurring event, and we are including recurrences, add date-qualifying occurrences.
      // For a non-recurring event, add it if it's date-qualified.
      for (let i = 0, {
          length
        } = candidateEvents; i < length; i++) {
        const e = candidateEvents[i];
        // For recurring events, add date-qualifying occurrences, not the base
        if (includeOccurrences && e.isRecurring) {
          events.push.apply(events, e.getOccurrencesForDateRange(startDate, endDate).filter(dateFilter));
        }
        // For ordinary events, add if it's date-qualified
        else if (dateFilter(e)) {
          events.push(e);
        }
      }
    }
    return filter ? events.filter(filter) : events;
  }
  /**
   * Internal implementation for {@link #function-getEvents} to use when using dateMap.
   * @private
   */
  getEventsAsMap({
    filter: passedFilter,
    date,
    resourceRecord,
    // Not supported yet. Will add if ever requested.
    startDate = date,
    // Events which intersect the startDate/endDate
    endDate,
    // will be returned
    startOnly,
    // Only events which start on each date will be returned
    includeOccurrences = true,
    // Interpolate occurrences into the returned event set
    dateMap,
    // Return a Map keyed by date each value being an array of events
    dayTime = null,
    storeFilterFn,
    // Private option. Select which date index to look up events in depending on the date
    // we are examining in the date iteration process. Some callers may want to use
    // different indices depending on the stage through the date iteration.
    // See Calendar package for usage.
    getDateIndex
  }) {
    var _dateMap;
    const me = this;
    // Convert `true` to a Map.
    if ((_dateMap = dateMap) !== null && _dateMap !== void 0 && _dateMap.clear) {
      dateMap.clear();
    } else {
      dateMap = new Map();
    }
    if (me.count) {
      const dateIndex = me.useDayIndex(dayTime),
        indexName = startOnly ? 'startDate' : 'date',
        recurringEvents = [],
        filter = e => (!passedFilter || passedFilter(e)) && (!storeFilterFn || storeFilterFn(e)),
        baseEventFilter = e => notRecurring(e) && filter(e);
      dayTime = dateIndex.dayTime; // dayTime=null becomes DayTime instance for midnight
      // We can't yet do this for just a resource.
      if (resourceRecord) {
        throw new Error('Querying for events for a resource and returning a date-keyed Map is not supported');
      } else {
        var _me$recurringEvents;
        // Add all recurring events which started before the end of our date range.
        // There are none in Gantt projects
        (_me$recurringEvents = me.recurringEvents) === null || _me$recurringEvents === void 0 ? void 0 : _me$recurringEvents.forEach(e => {
          if (dayTime.startOfDay(e.startDate) < endDate) {
            recurringEvents.push(e);
          }
        });
        // Iterate the date range, using the indices to find qualified events.
        for (const date = new Date(startDate); date < endDate; date.setDate(date.getDate() + 1)) {
          var _coincidingEvents;
          let [coincidingEvents, key] = dateIndex.get(getDateIndex ? getDateIndex(date) : indexName, date, true);
          // The index entry may be there, but it could be empty.
          if ((_coincidingEvents = coincidingEvents) !== null && _coincidingEvents !== void 0 && _coincidingEvents.size) {
            // Convert Set which index holds into an Array.
            // A recurring event doesn't go into the Map, its occurrences do.
            // Then filter by the passed filter and this Store's filter function
            // because events found from the date indices won't be filtered.
            coincidingEvents = [...coincidingEvents].filter(baseEventFilter);
            // Only create the entry for the day if there are events found
            if (coincidingEvents.length) {
              (dateMap.get(key) || dateMap.set(key, []).get(key)).push(...coincidingEvents);
            }
          }
        }
      }
      // Go through matching recurring events.
      for (let i = 0, {
          length
        } = recurringEvents; i < length; i++) {
        const e = recurringEvents[i],
          // For each recurring event, add occurrences if we are including occurrences else, add the base.
          // Then filter by the passed filter and this Store's filter function
          // because events found from the date indices won't be filtered.
          occurrences = (includeOccurrences ? e.getOccurrencesForDateRange(startDate, endDate) : [e]).filter(filter),
          lastDate = DateHelper.add(endDate, 1, 'day');
        // Add occurrences to dateMap
        for (let bucket, i = 0, {
            length
          } = occurrences; i < length; i++) {
          const occurrence = occurrences[i],
            date = dayTime.startOfDay(occurrence.startDate),
            indexName = getDateIndex ? getDateIndex(date) : startOnly ? 'startDate' : 'date',
            lastIntersectingDate = indexName === 'startDate' || !occurrence.durationMS ? DateHelper.add(date, 1, 'day') : DateHelper.min(occurrence.endDate || DateHelper.add(occurrence.startDate, occurrence.duration, occurrence.durationUnit), lastDate);
          // Loop through covered dates, adding to dateMap if required
          for (; date < lastIntersectingDate; date.setDate(date.getDate() + 1)) {
            const key = dayTime.dateKey(date);
            (bucket = dateMap.get(key)) || dateMap.set(key, bucket = []);
            bucket.push(occurrence);
          }
        }
      }
    }
    return dateMap;
  }
});

/**
 * @module Scheduler/data/util/EventDayIndex
 */
const
  // Maps an index name that can be requested to its storage property on the EventDayIndex instance:
  indexNameMap = {
    date: '_dateIndex',
    startDate: '_startDateIndex'
  },
  indexProps = Object.values(indexNameMap),
  emptyArray$2 = Object.freeze([]),
  {
    MILLIS_PER_DAY
  } = DayTime;
/**
 * This utility class is used by event stores to index events by their day (a "YYYY-MM-DD" value, also known as a
 * "date key"). This key is produced by a {@link Core.util.DayTime} instance. If two `DayTime` instances have a common
 * `startShift`, they can share an index.
 *
 * @internal
 */
class EventDayIndex {
  constructor(store, dayTime) {
    /**
     * The `DayTime` definition for this index. This is set to the initial DayTime instance but can be used for
     * any other {@link #function-register registered} `DayTime` instances since they all posses the same value for
     * `startShift`.
     *
     * This defaults to {@link Core.util.DayTime#property-MIDNIGHT-static}.
     * @member {Core.util.DayTime} dayTime
     * @readonly
     */
    this.dayTime = dayTime || DayTime.MIDNIGHT;
    /**
     * The owning store instance of this index.
     * @member {Scheduler.data.EventStore} store
     * @private
     * @readonly
     */
    this.store = store;
    /**
     * The `DayTime` instances {@link #function-register registered} with this index instance. As instances are
     * {@link #function-unregister unregistered} they are removed from this array. Once this array is empty, this
     * index can be discarded.
     * @member {Core.util.DayTime[]} users
     * @private
     */
    this.users = [this.dayTime];
  }
  /**
   * Adds an event record to the specified index (either "startDate" or "date") for a given `date`.
   * @param {String} indexName The index to which the event record is to be added (either "startDate" or "date").
   * @param {Date|Number} date A date for which the event record overlaps. The {@link Core.util.DayTime#function-dateKey}
   * method is used to convert this date to a "YYYY-MM-DD" key for the index.
   * @param {Scheduler.model.EventModel} eventRecord The event record.
   * @private
   */
  add(indexName, date, eventRecord) {
    const index = this[indexNameMap[indexName]],
      key = this.dayTime.dateKey(date),
      entry = index[key] || (index[key] = new Set());
    entry.add(eventRecord);
  }
  /**
   * Adds an event record to all indexes for all dates which the event overlaps.
   * @param {Scheduler.model.EventModel} eventRecord The event record.
   * @private
   */
  addEvent(eventRecord) {
    var _this$dayTime$startOf;
    let dateMS = (_this$dayTime$startOf = this.dayTime.startOfDay(eventRecord.startDate)) === null || _this$dayTime$startOf === void 0 ? void 0 : _this$dayTime$startOf.getTime(),
      endDateMS;
    if (dateMS) {
      var _eventRecord$endDate;
      endDateMS = ((_eventRecord$endDate = eventRecord.endDate) === null || _eventRecord$endDate === void 0 ? void 0 : _eventRecord$endDate.getTime()) ?? dateMS;
      this.add('startDate', dateMS, eventRecord);
      do {
        this.add('date', dateMS, eventRecord);
        dateMS += MILLIS_PER_DAY;
      } while (dateMS < endDateMS);
    }
  }
  /**
   * Clear this index.
   */
  clear() {
    indexProps.forEach(name => this[name] = Object.create(null));
  }
  /**
   * Returns an object that has properties named by the {@link Core.util.DayTime#function-dateKey} method, or the
   * array of event records if a `date` is specified, or the event record array and the date key in a 2-element array
   * if `returnKey` is `true`.
   * @param {String} indexName The name of the desired index (either 'date' or 'startDate').
   * @param {Number|Date} date The date as a `Date` or the millisecond UTC epoch. When passed, this method will return
   * the array of event records for this date.
   * @param {Boolean} [returnKey] Specify `true` to return the date key along with the event record array.
   * @returns {Object|Scheduler.model.EventModel[]}
   */
  get(indexName, date, returnKey) {
    // Date indices are created on first usage and after that kept up to date on changes
    !this.initialized && this.initialize();
    let ret = this[indexNameMap[indexName]],
      key;
    if (date) {
      key = this.dayTime.dateKey(date);
      ret = returnKey ? [ret[key], key] : ret[key];
    }
    return ret;
  }
  /**
   * Called when this index is first used. Once called, further store changes will be used to maintain this index.
   * @private
   */
  initialize() {
    this.initialized = true;
    this.clear();
    this.sync('splice', this.store.storage.allValues);
  }
  invalidate() {
    this.initialized = false;
    indexProps.forEach(name => this[name] = null);
  }
  /**
   * Returns `true` if the given `dayTime` matches this index.
   * @param {Core.util.DayTime} dayTime
   * @returns {Boolean}
   */
  matches(dayTime) {
    return this.dayTime.startShift === dayTime.startShift;
  }
  /**
   * Removes an event record from the specified index (either "startDate" or "date") for a given `date`.
   * @param {String} indexName The index to which the event record is to be removed (either "startDate" or "date").
   * @param {Date|Number} date A date for which the event record overlaps. The {@link Core.util.DayTime#function-dateKey}
   * method is used to convert this date to a "YYYY-MM-DD" key for the index.
   * @param {Scheduler.model.EventModel} eventRecord The event record.
   * @private
   */
  remove(indexName, date, eventRecord) {
    const index = this[indexNameMap[indexName]],
      key = this.dayTime.dateKey(date),
      entry = index[key];
    if (entry) {
      entry.delete(eventRecord);
    }
  }
  /**
   * Removes an event record from all indexes for all dates which the event overlaps.
   * @param {Scheduler.model.EventModel} eventRecord The event record.
   * @param {Date} startDate The start date for the event. This may be different from the `startDate` of the given
   * `eventRecord` when the event is rescheduled.
   * @param {Date} endDate The end date for the event. This may be different from the `endDate` of the given
   * `eventRecord` when the event is rescheduled.
   * @private
   */
  removeEvent(eventRecord, startDate, endDate) {
    var _this$dayTime$startOf2;
    let dateMS = (_this$dayTime$startOf2 = this.dayTime.startOfDay(startDate)) === null || _this$dayTime$startOf2 === void 0 ? void 0 : _this$dayTime$startOf2.getTime(),
      endDateMS;
    if (dateMS) {
      endDateMS = (endDate === null || endDate === void 0 ? void 0 : endDate.getTime()) ?? dateMS;
      this.remove('startDate', dateMS, eventRecord);
      do {
        this.remove('date', dateMS, eventRecord);
        dateMS += MILLIS_PER_DAY;
      } while (dateMS < endDateMS);
    }
  }
  sync(action, added, removed, replaced, wasSet) {
    var _wasSet$startDate, _wasSet$endDate;
    added = added || emptyArray$2;
    removed = removed || emptyArray$2;
    const me = this,
      addedCount = added.length,
      removedCount = removed.length,
      replacedCount = replaced === null || replaced === void 0 ? void 0 : replaced.length;
    let i, newEvent, outgoingEvent;
    if (!me.initialized) {
      return;
    }
    switch (action) {
      case 'clear':
        me.clear();
        break;
      // Add and remove
      case 'splice':
        // Handle replacement of records by instances with same ID
        if (replacedCount) {
          added = added.slice();
          removed = removed.slice();
          for (i = 0; i < replacedCount; i++) {
            removed.push(replaced[i][0]);
            added.push(replaced[i][1]);
          }
        }
        // Remove entries from indices
        if (removedCount) {
          for (i = 0; i < removedCount; i++) {
            outgoingEvent = removed[i];
            me.removeEvent(outgoingEvent, outgoingEvent.startDate, outgoingEvent.endDate);
          }
        }
        // Add entries to indices
        if (addedCount) {
          for (i = 0; i < addedCount; i++) {
            newEvent = added[i];
            // Can only be date-indexed if it's scheduled.
            // Also ignore parent events (likely using a Gantt project)
            if (newEvent.isScheduled && !newEvent.isParent) {
              me.addEvent(newEvent);
            }
          }
        }
        break;
      // invoked when the start or end changes so that the event can be re-indexed.
      case 'reschedule':
        outgoingEvent = added[0];
        me.removeEvent(outgoingEvent, ((_wasSet$startDate = wasSet.startDate) === null || _wasSet$startDate === void 0 ? void 0 : _wasSet$startDate.oldValue) || outgoingEvent.startDate, ((_wasSet$endDate = wasSet.endDate) === null || _wasSet$endDate === void 0 ? void 0 : _wasSet$endDate.oldValue) || outgoingEvent.endDate);
        // Now process as a splice with an add and no removes.
        me.sync('splice', added);
        break;
    }
  }
  /**
   * This method registers a `dayTime` instance with this index in the `users` array.
   * @param {Core.util.DayTime} dayTime The instance to register.
   */
  register(dayTime) {
    this.users.push(dayTime);
  }
  /**
   * This method unregisters a `dayTime` instance, removing it from the `users` array. This method returns `true` if
   * this was the last registered instance and this index is no longer needed.
   * @param {Core.util.DayTime} dayTime The instance to register.
   * @returns {Boolean}
   */
  unregister(dayTime) {
    const {
        users
      } = this,
      i = users.indexOf(dayTime);
    if (i > -1) {
      users.splice(i, 1);
    }
    return !users.length;
  }
}
// To avoid shape changes:
const proto = EventDayIndex.prototype;
indexProps.forEach(name => proto[name] = null);
proto.initialized = false;
EventDayIndex._$name = 'EventDayIndex';

/**
 * @module Scheduler/data/mixin/DayIndexMixin
 */
const {
  MIDNIGHT
} = DayTime;
/**
 * Mixing handling Calendars day indices.
 *
 * Consumed by EventStore in Scheduler & Scheduler Pro and TaskStore in Gantt.
 *
 * @mixin
 * @internal
 */
var DayIndexMixin = (Target => class DayIndexMixin extends Target {
  static $name = 'DayIndexMixin';
  construct(config) {
    super.construct(config);
    this.dayIndices = null;
  }
  //region Keeping index in sync
  // Override to syncIndices on initial load
  afterLoadData() {
    var _super$afterLoadData;
    this.syncIndices('splice', this.storage.allValues);
    (_super$afterLoadData = super.afterLoadData) === null || _super$afterLoadData === void 0 ? void 0 : _super$afterLoadData.call(this);
  }
  /**
   * Responds to mutations of the underlying storage Collection.
   *
   * Maintain indices for fast finding of events by date.
   * @param {Object} event
   * @private
   */
  onDataChange({
    action,
    added,
    removed,
    replaced
  }) {
    // Indices must be synced before responding to change
    this.syncIndices(action, added, removed, replaced);
    super.onDataChange(...arguments);
  }
  onDataReplaced(action, data) {
    // Indices must be synced before responding to change
    this.syncIndices('clear');
    this.syncIndices('splice', this.storage.values);
    super.onDataReplaced(action, data);
  }
  onModelChange(record, toSet, wasSet, silent, fromRelationUpdate) {
    // Ensure by-date indices are up to date.
    if ('startDate' in wasSet || 'endDate' in wasSet) {
      this.syncIndices('reschedule', [record], null, null, wasSet);
    }
    super.onModelChange(...arguments);
  }
  //endregion
  //region Index
  /**
   * Invalidates associated day indices.
   * @internal
   */
  invalidateDayIndices() {
    var _this$dayIndices;
    (_this$dayIndices = this.dayIndices) === null || _this$dayIndices === void 0 ? void 0 : _this$dayIndices.forEach(dayIndex => dayIndex.invalidate());
  }
  /**
   * Registers a `DayTime` instance, creating an `EventDayIndex` for each distinct `startShift`. This index is
   * maintained until all instances with a matching `startShift` are {@link #function-unregisterDayIndex unregistered}.
   * @param {Core.util.DayTime} dayTime The instance to register.
   * @internal
   * @category Indexing
   */
  registerDayIndex(dayTime) {
    const me = this,
      dayIndices = me.dayIndices || (me.dayIndices = []);
    let dayIndex, i;
    for (i = 0; !dayIndex && i < dayIndices.length; ++i) {
      if (dayIndices[i].matches(dayTime)) {
        (dayIndex = dayIndices[i]).register(dayTime);
      }
    }
    !dayIndex && dayIndices.push(dayIndex = new EventDayIndex(me, dayTime));
    return dayIndex;
  }
  syncIndices(...args) {
    var _this$dayIndices2;
    (_this$dayIndices2 = this.dayIndices) === null || _this$dayIndices2 === void 0 ? void 0 : _this$dayIndices2.forEach(dayIndex => dayIndex.sync(...args));
  }
  /**
   * Removes a registered `DayTime` instance. If this is the last instance registered to an `EventDayIndex`, that
   * index is removed.
   * @param {Core.util.DayTime} dayTime The instance to unregister.
   * @internal
   * @category Indexing
   */
  unregisterDayIndex(dayTime) {
    const me = this,
      {
        dayIndices
      } = me;
    for /* empty */
    (let i = dayIndices === null || dayIndices === void 0 ? void 0 : dayIndices.length; i-- > 0;) {
      if (dayIndices[i].matches(dayTime)) {
        if (dayIndices[i].unregister(dayTime)) {
          dayIndices.splice(i, 1);
        }
        break;
      }
    }
  }
  /**
   * Returns the `EventDayIndex` to use for the given `DayTime` instance. This may be the primary instance or a
   * child instance created by {@link #function-registerDayIndex}.
   * @param {Core.util.DayTime} dayTime The `DayTime` of the desired index.
   * @returns {Scheduler.data.util.EventDayIndex}
   * @private
   * @category Indexing
   */
  useDayIndex(dayTime) {
    const me = this,
      {
        dayIndices
      } = me;
    dayTime = dayTime || MIDNIGHT;
    for (let i = 0; dayIndices && i < dayIndices.length; ++i) {
      if (dayIndices[i].matches(dayTime)) {
        return dayIndices[i];
      }
    }
    if (dayTime.startShift) {
      throw new Error(`No day index registered for ${dayTime} on ${me.id}`);
    }
    return me.registerDayIndex(MIDNIGHT);
  }
  //endregion
});

/**
 * @module Scheduler/data/mixin/SharedEventStoreMixin
 */
/**
 * This is a mixin, containing functionality related to managing events.
 *
 * It is consumed by the regular {@link Scheduler.data.EventStore} class and Scheduler Pros counterpart.
 *
 * @mixin
 */
var SharedEventStoreMixin = (Target => class SharedEventStoreMixin extends Target {
  static get $name() {
    return 'SharedEventStoreMixin';
  }
  /**
   * Add events to the store.
   *
   * NOTE: Dates, durations and references (assignments, resources) on the events are determined async by a calculation
   * engine. Thus they cannot be directly accessed after using this function.
   *
   * For example:
   *
   * ```javascript
   * eventStore.add({ startDate, duration });
   * // endDate is not yet calculated
   * ```
   *
   * To guarantee data is in a calculated state, wait for calculations for finish:
   *
   * ```javascript
   * eventStore.add({ startDate, duration });
   * await eventStore.project.commitAsync();
   * // endDate is calculated
   * ```
   *
   * Alternatively use `addAsync()` instead:
   *
   * ```javascript
   * await eventStore.addAsync({ startDate, duration });
   * // endDate is calculated
   * ```
   *
   * @param {Scheduler.model.EventModel|Scheduler.model.EventModel[]|EventModelConfig|EventModelConfig[]} records
   * Array of records/data or a single record/data to add to store
   * @param {Boolean} [silent] Specify `true` to suppress events
   * @returns {Scheduler.model.EventModel[]} Added records
   * @function add
   * @category CRUD
   */
  /**
   * Add events to the store and triggers calculations directly after. Await this function to have up to date data on
   * the added events.
   *
   * ```javascript
   * await eventStore.addAsync({ startDate, duration });
   * // endDate is calculated
   * ```
   *
   * @param {Scheduler.model.EventModel|Scheduler.model.EventModel[]|EventModelConfig|EventModelConfig[]} records
   * Array of records/data or a single record/data to add to store
   * @param {Boolean} [silent] Specify `true` to suppress events
   * @returns {Scheduler.model.EventModel[]} Added records
   * @function addAsync
   * @category CRUD
   * @async
   */
  /**
   * Applies a new dataset to the EventStore. Use it to plug externally fetched data into the store.
   *
   * NOTE: Dates, durations and relations (assignments, resources) on the events are determined async by a calculation
   * engine. Thus they cannot be directly accessed after assigning the new dataset.
   *
   * For example:
   *
   * ```javascript
   * eventStore.data = [{ startDate, duration }];
   * // eventStore.first.endDate is not yet calculated
   * ```
   *
   * To guarantee data is in a calculated state, wait for calculations for finish:
   *
   * ```javascript
   * eventStore.data = [{ startDate, duration }];
   * await eventStore.project.commitAsync();
   * // eventStore.first.endDate is calculated
   * ```
   *
   * Alternatively use `loadDataAsync()` instead:
   *
   * ```javascript
   * await eventStore.loadDataAsync([{ startDate, duration }]);
   * // eventStore.first.endDate is calculated
   * ```
   *
   * @member {EventModelConfig[]} data
   * @category Records
   */
  /**
   * Applies a new dataset to the EventStore and triggers calculations directly after. Use it to plug externally
   * fetched data into the store.
   *
   * ```javascript
   * await eventStore.loadDataAsync([{ startDate, duration }]);
   * // eventStore.first.endDate is calculated
   * ```
   *
   * @param {EventModelConfig[]} data Array of EventModel data objects
   * @function loadDataAsync
   * @category CRUD
   * @async
   */
  static get defaultConfig() {
    return {
      /**
       * CrudManager must load stores in the correct order. Lowest first.
       * @private
       */
      loadPriority: 100,
      /**
       * CrudManager must sync stores in the correct order. Lowest first.
       * @private
       */
      syncPriority: 200,
      storeId: 'events',
      /**
       * Configure with `true` to also remove the event when removing the last assignment from the linked
       * AssignmentStore. This config has not effect when using EventStore in legacy `resourceId`-mode.
       * @config {Boolean}
       * @default
       * @category Common
       */
      removeUnassignedEvent: true,
      /**
       * Configure with `true` to force single-resource mode, an event can only be assigned to a single resource.
       * If not provided, the mode will be inferred from
       *
       * 1. presence of an assignment store (i.e. multi-assignment)
       * 2. presence of `resourceId` in the event store data (i.e. single assignment mode)
       * @config {Boolean}
       * @category Common
       */
      singleAssignment: null
    };
  }
  /**
   * Class used to represent records. Defaults to class EventModel.
   * @member {Scheduler.model.EventModel} modelClass
   * @typings {typeof EventModel}
   * @category Records
   */
  construct(config) {
    super.construct(config, true);
    if (this.singleAssignment) {
      this.usesSingleAssignment = true;
    }
    if (!this.modelClass.isEventModel) {
      throw new Error('The model for the EventStore must subclass EventModel');
    }
  }
  /**
   * Appends a new record to the store
   * @param {Scheduler.model.EventModel} record The record to append to the store
   * @category CRUD
   */
  append(record) {
    return this.add(record);
  }
  //region Project
  get project() {
    return super.project;
  }
  set project(project) {
    super.project = project;
    this.detachListeners('project');
    if (project) {
      var _project$assignmentSt;
      // Project already has AssignmentStore instance? Attach to it.
      if ((_project$assignmentSt = project.assignmentStore) !== null && _project$assignmentSt !== void 0 && _project$assignmentSt.isAssignmentStore) {
        this.attachToAssignmentStore(project.assignmentStore);
      }
      // Accessing assignmentStore would trigger `assignmentStoreChange` event on the project, so we set up
      // the listener after
      project.ion({
        name: 'project',
        assignmentStoreChange: 'onProjectAssignmentStoreChange',
        thisObj: this,
        prio: 200 // Before UI updates
      });
    }
  }
  //endregion
  //region Single assignment
  get usesSingleAssignment() {
    if (this.isChained) {
      return this.masterStore.usesSingleAssignment;
    }
    return this._usesSingleAssignment;
  }
  set usesSingleAssignment(value) {
    this._usesSingleAssignment = value;
  }
  processRecords(eventRecords) {
    var _this$stm;
    const {
        assignmentStore
      } = this,
      assignmentsToAdd = [];
    // Same as on `joinRecordsToStore`, when adding a number of event records CoreEventMixin#joinProject method
    // will clear/rebuild cache in a loop. We raise this flag to skip invalidating assignment store indices for the time
    // we are joining records to the store. When they're added and indices are read, we will invalidate them.
    if (assignmentStore) {
      assignmentStore.skipInvalidateIndices = true;
    }
    eventRecords = super.processRecords(eventRecords, assignmentStore && !((_this$stm = this.stm) !== null && _this$stm !== void 0 && _this$stm.isRestoring) && (eventRecord => {
      // AssignmentStore found, add an assignment to it if this is not a dataset operation
      const resourceId = eventRecord.get('resourceId');
      if (!eventRecord.reassignedFromReplace && resourceId != null) {
        // Check if the event is already assigned to the resource, though it's not in the event store.
        // It could happen when you remove an event, so both event and assignment records are removed,
        // then you "undo" the action and the assignment is restored before the event is restored.
        if (!assignmentStore.includesAssignment(eventRecord.id, resourceId)) {
          // Cannot use `event.assign(resourceId)` since event is not part of store yet
          // Using a bit shorter generated id to not look so ugly in DOM
          assignmentsToAdd.push({
            id: assignmentStore.modelClass.generateId(''),
            resourceId,
            eventId: eventRecord.id
          });
        }
      }
      // clear flag
      eventRecord.reassignedFromReplace = false;
    }) || undefined);
    if (assignmentStore) {
      assignmentStore.storage.invalidateIndices();
      assignmentStore.skipInvalidateIndices = false;
      assignmentStore.add(assignmentsToAdd);
    }
    return eventRecords;
  }
  joinRecordsToStore(records) {
    const {
      assignmentStore
    } = this;
    if (assignmentStore) {
      // When adding a number of event records CoreEventMixin#joinProject method will clear/rebuild cache in a loop.
      // We raise this flag to skip invalidating assignment store indices for the time we are joining records to
      // the store. When they're added and indices are read, we will invalidate them.
      assignmentStore.skipInvalidateIndices = true;
      super.joinRecordsToStore(records);
      assignmentStore.storage.invalidateIndices();
      assignmentStore.skipInvalidateIndices = false;
    } else {
      super.joinRecordsToStore(records);
    }
  }
  processRecord(eventRecord, isDataset = false) {
    eventRecord = super.processRecord(eventRecord, isDataset);
    const resourceId = eventRecord.get('resourceId');
    if (resourceId != null && !eventRecord.meta.skipEnforcingSingleAssignment) {
      const me = this,
        {
          assignmentStore
        } = me,
        existingRecord = me.getById(eventRecord.id),
        isReplacing = existingRecord && existingRecord !== eventRecord && !isDataset;
      // Replacing an existing event, repoint the resource of its assignment
      // (already repointed to the new event by engine in EventStoreMixin)
      if (isReplacing) {
        // Have to look assignment up on store, removed by engine in super call above
        const assignment = assignmentStore.find(e => e.eventId === eventRecord.id);
        if (assignment) {
          assignment.resource = resourceId;
          eventRecord.reassignedFromReplace = true;
        }
      }
      // No AssignmentStore assigned yet, need to process when that happens. Or if it is a dataset operation,
      // processing will happen at the end of it to not add individual assignment (bad for performance)
      else {
        me.$processResourceIds = true;
      }
      // Flag that we have been loaded using resourceId, checked by CrudManager to exclude the internal
      // AssignmentStore from sync
      me.usesSingleAssignment = true;
    }
    return eventRecord;
  }
  processResourceIds() {
    var _me$crudManager, _me$project;
    const me = this,
      // When used in a standalone CrudManager, there is no direct link to the assignment store
      assignmentStore = me.assignmentStore ?? ((_me$crudManager = me.crudManager) === null || _me$crudManager === void 0 ? void 0 : _me$crudManager.assignmentStore);
    if (me.$processResourceIds && assignmentStore !== null && assignmentStore !== void 0 && assignmentStore.isAssignmentStore && !((_me$project = me.project) !== null && _me$project !== void 0 && _me$project.isSharingAssignmentStore && me.isChained)) {
      const assignments = [];
      // resourceIds used during initialization, convert into assignments
      me.forEach(eventRecord => {
        const {
          resourceId,
          id: eventId
        } = eventRecord;
        if (resourceId != null) {
          // Using a bit shorter generated id to not look so ugly in DOM
          assignments.push({
            id: assignmentStore.modelClass.generateId(''),
            resourceId,
            eventId
          });
        }
      });
      // Disable as much as possible, since we are in full control of this store when using single assignment mode
      assignmentStore.useRawData = {
        disableDefaultValue: true,
        disableDuplicateIdCheck: true,
        disableTypeConversion: true
      };
      // Flag that throws in AssignmentStore if data is loaded some other way when using single assignment
      assignmentStore.usesSingleAssignment = false;
      // These assignments all use generated ids, and are not meant to be searialized anyway so bypass check
      assignmentStore.verifyNoGeneratedIds = false;
      assignmentStore.data = assignments;
      assignmentStore.usesSingleAssignment = true;
      me.$processResourceIds = false;
    }
  }
  loadData() {
    super.loadData(...arguments);
    this.processResourceIds();
  }
  // Optionally remove unassigned events
  onBeforeRemoveAssignment({
    records
  }) {
    var _me$stm;
    const me = this;
    if (me.removeUnassignedEvent && !me.isRemoving && !me.isSettingData && !((_me$stm = me.stm) !== null && _me$stm !== void 0 && _me$stm.isRestoring) && !me.usesSingleAssignment &&
    // Do not remove unassigned events when syncing data, new assignments etc. might be synced afterwards
    !me.assignmentStore.isSyncingDataOnLoad && !me.resourceStore.isSyncingDataOnLoad) {
      const toRemove = new Set();
      // Collect all events that are unassigned after the remove
      records.forEach(assignmentRecord => {
        const {
          event
        } = assignmentRecord;
        // Assignment might not have an event or the event might already be removed
        if (event && !event.isRemoved && event.assignments.every(a => records.includes(a))) {
          toRemove.add(event);
        }
      });
      // And remove them
      if (toRemove.size) {
        me.remove([...toRemove]);
      }
    }
  }
  onProjectAssignmentStoreChange({
    store
  }) {
    this.attachToAssignmentStore(store);
  }
  attachToAssignmentStore(assignmentStore) {
    const me = this;
    me.detachListeners('assignmentStore');
    if (assignmentStore) {
      me.processResourceIds();
      assignmentStore.ion({
        name: 'assignmentStore',
        // Adding an assignment in single assignment mode should set events resourceId if needed
        addPreCommit({
          records
        }) {
          if (me.usesSingleAssignment && !me.isSettingData && !me.isAssigning) {
            records.forEach(assignment => {
              const {
                event
              } = assignment;
              if (event !== null && event !== void 0 && event.isEvent && event.resourceId !== assignment.resourceId) {
                event.meta.isAssigning = true;
                event.set('resourceId', assignment.resourceId);
                event.meta.isAssigning = false;
              }
            });
          }
        },
        // Called both for remove and removeAll
        beforeRemove: 'onBeforeRemoveAssignment',
        // Removing an assignment in single assignment mode should set events resourceId to null
        removePreCommit({
          records
        }) {
          if (me.usesSingleAssignment) {
            records.forEach(assignment => {
              var _me$getById;
              // With engine link to event is already broken when we get here, hence the lookup
              (_me$getById = me.getById(assignment.eventId)) === null || _me$getById === void 0 ? void 0 : _me$getById.set('resourceId', null);
            });
          }
        },
        removeAllPreCommit() {
          if (me.usesSingleAssignment && !me.isSettingData) {
            me.allRecords.forEach(eventRecord => eventRecord.set('resourceId', null));
          }
        },
        // Keep events resourceId in sync with assignment on changes in single assignment mode
        update({
          record,
          changes
        }) {
          if (me.usesSingleAssignment && 'resourceId' in changes) {
            const {
              event
            } = record;
            event.meta.isAssigning = true;
            event.set('resourceId', changes.resourceId.value);
            event.meta.isAssigning = false;
          }
        },
        thisObj: me
      });
    }
  }
  set data(data) {
    this.isSettingData = true;
    // When using single assignment, remove all assignments when loading a new set of events.
    // Don't do it when filling a chained store, assignments are for the master store
    if (this.usesSingleAssignment && !this.syncDataOnLoad && !this.isChained) {
      this.assignmentStore.removeAll(true);
    }
    super.data = data;
    this.isSettingData = false;
  }
  // Override trigger to decorate update/change events with a flag if resourceId was the only thing changed, in which
  // case the change most likely can be ignored since the assignment will also change
  trigger(eventName, params) {
    var _this$stm2;
    const {
      changes
    } = params || {};
    // https://github.com/bryntum/support/issues/6610
    // test: SchedulerPro/tests/data/UndoRedo.t.js, "Should refresh the view after undoing the event copy-paste"
    // can not ignore this event when stm is restoring, because of the edge case in that ticket
    if (changes && 'resourceId' in changes && Object.keys(changes).length === 1 && !((_this$stm2 = this.stm) !== null && _this$stm2 !== void 0 && _this$stm2.isRestoring)) {
      params.isAssign = true;
    }
    return super.trigger(...arguments);
  }
  remove(records, ...args) {
    const result = super.remove(records, ...args);
    // Make sure assignment is removed with event when using single assignment
    if (result.length && this.usesSingleAssignment) {
      for (const eventRecord of result) {
        if (!eventRecord.isOccurrence) {
          var _ref, _this$crudManager;
          (_ref = this.assignmentStore || ((_this$crudManager = this.crudManager) === null || _this$crudManager === void 0 ? void 0 : _this$crudManager.assignmentStore)) === null || _ref === void 0 ? void 0 : _ref.remove(eventRecord.assignments, true);
        }
      }
    }
    return result;
  }
  //endregion
});

/**
 * @module Scheduler/data/mixin/RecurringTimeSpansMixin
 */
const emptyArray$1 = Object.freeze([]);
/**
 * This mixin provides recurrence functionality to a store containing {@link Scheduler.model.TimeSpan TimeSpan} models.
 * Normally you don't need to interact with this mixin directly.
 * @mixin
 */
var RecurringTimeSpansMixin = (Target => class RecurringTimeSpansMixin extends (Target || Base) {
  static get $name() {
    return 'RecurringTimeSpansMixin';
  }
  construct(...args) {
    const me = this;
    // We store all generated occurrences keyed by `_generated_${recurringTimeSpan.id}:${occurrenceStartDate}`
    // So that when asked to generate an occurrence for a date, an already generated one can be returned.
    me.globalOccurrences = new Map();
    // All recurring events added to the store are accessible through this Set. It's used
    // to generate occurrences.
    me.recurringEvents = new Set();
    super.construct(...args);
  }
  // Override to refreshRecurringEventsCache on initial load
  afterLoadData() {
    // All cached occurrences are now potentially invalid.
    // A store reload might imply any number of changes which invalidate any occurrence.
    this.globalOccurrences.clear();
    // Clear and rebuild the recurring events cache
    this.refreshRecurringEventsCache('clear');
    this.refreshRecurringEventsCache('splice', this.storage.allValues);
    super.afterLoadData && super.afterLoadData();
  }
  /**
   * Responds to mutations of the underlying storage Collection.
   *
   * Maintain indices for fast finding of events by date.
   * @param {Object} event
   * @private
   */
  onDataChange({
    action,
    added,
    removed,
    replaced
  }) {
    // Recurring events cache must be refreshed before responding to change
    this.refreshRecurringEventsCache(action, added, removed, replaced);
    super.onDataChange(...arguments);
  }
  refreshRecurringEventsCache(action, added = emptyArray$1, removed = emptyArray$1, replaced) {
    const me = this,
      {
        recurringEvents
      } = me,
      replacedCount = replaced === null || replaced === void 0 ? void 0 : replaced.length;
    switch (action) {
      case 'clear':
        recurringEvents.clear();
        break;
      // Add and remove
      case 'splice':
        {
          // Handle replacement of records by instances with same ID
          if (replacedCount) {
            added = added.slice();
            removed = removed.slice();
            for (let i = 0; i < replacedCount; i++) {
              removed.push(replaced[i][0]);
              added.push(replaced[i][1]);
            }
          }
          const addedCount = added.length,
            removedCount = removed.length;
          // Track the recurring events we contain
          if (removedCount && recurringEvents.size) {
            for (let i = 0; i < removedCount; i++) {
              // If it's being removed, remove it from the recurring events cache.
              // If it's not a recurring event, it doesn't matter, it won't be in there.
              recurringEvents.delete(removed[i]);
            }
          }
          // Track the recurring events we contain
          if (addedCount) {
            for (let i = 0; i < addedCount; i++) {
              const newEvent = added[i];
              // Allow easy access to recurring events
              if (newEvent.isRecurring) {
                recurringEvents.add(newEvent);
              }
            }
          }
          break;
        }
    }
  }
  getById(id) {
    let result = super.getById(id);
    // If the id is not found in the Store, then it could be one of our generated occurrences
    if (!result) {
      result = this.globalOccurrences.get(this.modelClass.asId(id));
    }
    return result;
  }
  onModelChange(record, toSet, wasSet, silent, fromRelationUpdate) {
    const isRecurrenceRelatedFieldChange = !silent && this.isRecurrenceRelatedFieldChange(record, wasSet);
    // If this is the base of a recurring sequence, then any reactors to events from
    // the super call must regenerate occurrences, so must be done at top.
    // If silent is true, occurrences won't be recalculated. Do not remove occurrences from cache in such case.
    if (isRecurrenceRelatedFieldChange) {
      record.removeOccurrences();
    }
    super.onModelChange(...arguments);
    // If this is the base of a recurring sequence, then the EventStore must
    // trigger a refresh event so that UIs refresh themselves.
    // This could be at the tail end of the creation of an exception
    // or a new recurring base.
    if (isRecurrenceRelatedFieldChange) {
      const event = {
        action: 'batch',
        records: this.storage.values
      };
      this.trigger('refresh', event);
      this.trigger('change', event);
    }
  }
  /**
   * The method restricts which field modifications should trigger timespan occurrences building.
   * By default, any field change of a recurring timespan causes the rebuilding.
   * @param  {Scheduler.model.TimeSpan} timeSpan The modified timespan.
   * @param  {Object} wasSet Object containing the change set.
   * @returns {Boolean} `True` if the fields modification should trigger the timespan occurrences rebuilding.
   * @internal
   * @category Recurrence
   */
  isRecurrenceRelatedFieldChange(timeSpan, wasSet) {
    return timeSpan.isRecurring || 'recurrenceRule' in wasSet;
  }
  /**
   * Builds occurrences for the provided timespan across the provided date range.
   * @private
   * @category Recurrence
   */
  getOccurrencesForTimeSpan(timeSpan, startDate, endDate) {
    const result = [];
    if (timeSpan.isRecurring) {
      timeSpan.recurrence.forEachOccurrence(startDate, endDate, r => result.push(r));
    }
    return result;
  }
  set data(data) {
    // All cached occurrences are now invalid with a new dataset
    this.globalOccurrences.clear();
    super.data = data;
  }
  /**
   * Returns all the recurring timespans.
   * @returns {Scheduler.model.TimeSpan[]} Array of recurring events.
   * @category Recurrence
   */
  getRecurringTimeSpans() {
    return [...this.recurringEvents];
  }
});

/**
 * @module Scheduler/data/mixin/RecurringEventsMixin
 */
/**
 * This mixin class provides recurrence functionality to the {@link Scheduler.data.EventStore event store}.
 * @extends Scheduler/data/mixin/RecurringTimeSpansMixin
 * @mixin
 */
var RecurringEventsMixin = (Target => class RecurringEventsMixin extends RecurringTimeSpansMixin(Target || Base) {
  static get $name() {
    return 'RecurringEventsMixin';
  }
  /**
   * Returns all the recurring events.
   *
   * **An alias for ** {@link Scheduler.data.mixin.RecurringTimeSpansMixin#function-getRecurringTimeSpans} method.
   *
   * @returns {Scheduler.model.EventModel[]} Array of recurring events.
   * @category Recurrence
   */
  getRecurringEvents() {
    return this.getRecurringTimeSpans();
  }
  isEventPersistable(event) {
    // occurrences are not persistable
    return super.isEventPersistable(event) && (!event.supportsRecurring || !event.isOccurrence);
  }
});

/**
 * @module Scheduler/model/mixin/TimeZonedDatesMixin
 */
const dateFieldsToConvert = {
  startDate: 1,
  endDate: 1,
  constraintDate: 1,
  deadlineDate: 1
};
/**
 * This mixin class overrides default Model functionality to provide support for time zone converted dates
 * @mixin
 * @mixinbase TimeSpan
 */
var TimeZonedDatesMixin = (Target => class TimeZonedDatesMixin extends (Target || TimeSpan) {
  static $name = 'TimeZonedDatesMixin';
  static fields = [
  /**
   * The current timeZone this record is converted to. Used internally to keep track of time zone conversions.
   *
   * Can also be used to create a new record with dates in a specific non-local timezone. That is useful for
   * example when replacing a store dataset. That would be interpreted as a new load, and all dates would be
   * converted to the configured timezone.
   *
   * For more information about timezone conversion, se {@link Scheduler.model.ProjectModel#config-timeZone}.
   *
   * This field will not {@link Core.data.field.DataField#config-persist} by default.
   *
   * @field {String|Number} timeZone
   * @category Advanced
   */
  {
    name: 'timeZone',
    persist: false
  }];
  get timeZone() {
    return this.getData('timeZone');
  }
  set timeZone(timeZone) {
    this.setData('timeZone', timeZone);
  }
  setLocalDate(field, date) {
    this.set(field, this.timeZone != null ? TimeZoneHelper.toTimeZone(date, this.timeZone) : date, true);
    // Need to set data to fool engine that the dates havn't changed
    this.data[field] = this[field];
  }
  getLocalDate(field) {
    if (this.timeZone != null && this[field]) {
      return TimeZoneHelper.fromTimeZone(this[field], this.timeZone);
    }
    return this[field];
  }
  applyChangeset(rawChanges) {
    // When a sync response arrives from backend, the data will need to be converted to time zone before applied.
    if (this.timeZone != null) {
      for (const field in dateFieldsToConvert) {
        if (rawChanges[field]) {
          this.setLocalDate(field, new Date(rawChanges[field]));
          delete rawChanges[field];
        }
      }
    }
    return super.applyChangeset(...arguments);
  }
  getFieldPersistentValue(field) {
    if (this.timeZone != null) {
      const fieldName = (field === null || field === void 0 ? void 0 : field.field) ?? (field === null || field === void 0 ? void 0 : field.name) ?? field;
      // Used when saving/syncing. Returns local system dates
      if (dateFieldsToConvert[fieldName]) {
        return this.getLocalDate(fieldName);
      }
    }
    return super.getFieldPersistentValue(field);
  }
  // Converts current record into a timeZone
  convertToTimeZone(timeZone) {
    const me = this,
      metaModified = {
        ...me.meta.modified
      },
      convertFields = {
        ...dateFieldsToConvert
      };
    // Do not convert start and end dates on task unless manually scheduled
    if (me.isTask && !me.manuallyScheduled) {
      delete convertFields.startDate;
      delete convertFields.endDate;
    }
    // Collect values
    for (const field in convertFields) {
      // Only convert if field has value
      if (me[field] != null) {
        convertFields[field] = me[field];
        // If already converted, restore to local system time zone
        if (me.timeZone != null) {
          convertFields[field] = me.getLocalDate(field);
          // Restore value in meta modified as well
          if (metaModified[field]) {
            metaModified[field] = TimeZoneHelper.fromTimeZone(metaModified[field], me.timeZone);
          }
        }
      } else {
        delete convertFields[field];
      }
    }
    // Change time zone
    me.timeZone = timeZone;
    // Set values
    for (const field in convertFields) {
      // Convert and set field date silently
      me.setLocalDate(field, convertFields[field], false);
      convertFields[field] = 1; // For clearing changes below
      // Convert value in meta modified as well
      if (me.timeZone != null && metaModified[field]) {
        metaModified[field] = TimeZoneHelper.toTimeZone(metaModified[field], me.timeZone);
      }
    }
    // Clear modification metadata
    me.clearChanges(true, true, convertFields);
    // If old modification metadata, restore them to record and store
    if (!ObjectHelper.isEmpty(metaModified)) {
      me.meta.modified = metaModified;
      me.stores.forEach(store => store.modified.add(me));
    }
  }
});

/**
 * @module Scheduler/model/TimeSpan
 */
/**
 * This class represent a simple date range. It is being used in various subclasses and plugins which operate on date ranges.
 *
 * It's a subclass of {@link Core.data.Model}.
 * Please refer to documentation of those classes to become familiar with the base interface of this class.
 *
 * A TimeSpan has the following fields:
 *
 * - {@link #field-startDate}    - start date of the task in the ISO 8601 format
 * - {@link #field-endDate}      - end date of the task in the ISO 8601 format (not inclusive)
 * - {@link #field-duration}     - duration, time between start date and end date
 * - {@link #field-durationUnit} - unit used to express the duration
 * - {@link #field-name}         - an optional name of the range
 * - {@link #field-cls}          - an optional CSS class to be associated with the range.
 *
 * The data source of any field can be customized in the subclass. Please refer to {@link Core.data.Model} for details. To specify
 * another date format:
 *
 * ```javascript
 * class MyTimeSpan extends TimeSpan {
 *   static get fields() {
 *      { name: 'startDate', type: 'date', dateFormat: 'DD/MM/YY' }
 *   }
 * }
 * ```
 *
 * @extends Core/data/Model
 * @mixes Scheduler/model/mixin/TimeZonedDatesMixin
 */
class TimeSpan extends Model.mixin(TimeZonedDatesMixin) {
  static get $name() {
    return 'TimeSpan';
  }
  //region Field definitions
  static get fields() {
    return [
    /**
     * The start date of a time span (or Event / Task).
     *
     * Uses {@link Core/helper/DateHelper#property-defaultFormat-static DateHelper.defaultFormat} to convert a
     * supplied string to a Date. To specify another format, either change that setting or subclass TimeSpan and
     * change the dateFormat for this field.
     *
     * Note that the field always returns a `Date`.
     *
     * @field {Date} startDate
     * @accepts {String|Date}
     * @category Scheduling
     */
    {
      name: 'startDate',
      type: 'date'
    },
    /**
     * The end date of a time span (or Event / Task).
     *
     * Uses {@link Core/helper/DateHelper#property-defaultFormat-static DateHelper.defaultFormat} to convert a
     * supplied string to a Date. To specify another format, either change that setting or subclass TimeSpan and
     * change the dateFormat for this field.
     *
     * Note that the field always returns a `Date`.
     *
     * @field {Date} endDate
     * @accepts {String|Date}
     * @category Scheduling
     */
    {
      name: 'endDate',
      type: 'date'
    },
    /**
     * The numeric part of the timespan's duration (the number of units).
     * @field {Number} duration
     * @category Scheduling
     */
    {
      name: 'duration',
      type: 'number',
      allowNull: true,
      internal: true
    },
    /**
     * The unit part of the TimeSpan duration, defaults to "d" (days). Valid values are:
     *
     * - "millisecond" - Milliseconds
     * - "second" - Seconds
     * - "minute" - Minutes
     * - "hour" - Hours
     * - "day" - Days
     * - "week" - Weeks
     * - "month" - Months
     * - "quarter" - Quarters
     * - "year"- Years
     *
     * This field is readonly after creation, to change durationUnit use #setDuration().
     * @field {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'} durationUnit
     * @category Scheduling
     */
    {
      type: 'durationunit',
      name: 'durationUnit',
      defaultValue: 'd',
      internal: true
    },
    /**
     * Calculated field which encapsulates the duration's magnitude and unit. This field will not be persisted,
     * setting it will update the {@link #field-duration} and {@link #field-durationUnit} fields.
     *
     * @field {DurationConfig|Core.data.Duration} fullDuration
     * @category Scheduling
     */
    {
      name: 'fullDuration',
      persist: false,
      column: {
        type: 'duration'
      }
    },
    /**
     * An encapsulation of the CSS classes to add to the rendered time span element.
     *
     * Always returns a {@link Core.helper.util.DomClassList}, but may still be treated as a string. For
     * granular control of adding and removing individual classes, it is recommended to use the
     * {@link Core.helper.util.DomClassList} API.
     *
     * @field {Core.helper.util.DomClassList} cls
     * @accepts {Core.helper.util.DomClassList|String|String[]|Object}
     *
     * @category Styling
     */
    {
      name: 'cls',
      defaultValue: '',
      internal: true
    },
    /**
     * CSS class specifying an icon to apply to the rendered time span element.
     * **Note**: In case event is a milestone, using `iconCls` with dependency feature might slightly decrease
     * performance because feature will refer to the DOM to get exact size of the element.
     * @field {String} iconCls
     * @category Styling
     */
    {
      name: 'iconCls',
      internal: true
    },
    /**
     * A CSS style string (applied to `style.cssText`) or object (applied to `style`)
     * ```
     * record.style = 'color: red;font-weight: 800';
     * ```
     *
     * @field {String} style
     * @category Styling
     */
    {
      name: 'style',
      type: 'object',
      internal: true
    },
    /**
     * The name of the time span (or Event / Task)
     * @field {String} name
     * @category Common
     */
    {
      name: 'name',
      type: 'string',
      defaultValue: ''
    }];
  }
  //endregion
  //region Init
  construct(data, ...args) {
    // fullDuration is a "calculated field", but convenient to allow supplying it in the data
    if (data !== null && data !== void 0 && data.fullDuration) {
      const {
        magnitude,
        unit
      } = data.fullDuration;
      data.duration = magnitude;
      data.unit = unit;
      delete data.fullDuration;
    }
    super.construct(data, ...args);
    this.normalize();
  }
  /**
   * Returns the event store this event is part of, if any.
   *
   * @property {Scheduler.data.EventStore}
   * @readonly
   * @category Misc
   */
  get eventStore() {
    const me = this;
    // If we are an occurrence, return our base recurring event's store
    if (me.isOccurrence) {
      return me.recurringTimeSpan.eventStore;
    }
    if (!me._eventStore) {
      var _me$stores;
      me._eventStore = (_me$stores = me.stores) === null || _me$stores === void 0 ? void 0 : _me$stores.find(s => s.isEventStore);
    }
    return me._eventStore;
  }
  normalize() {
    const me = this,
      {
        startDate,
        endDate,
        duration,
        durationUnit
      } = me,
      hasDuration = duration != null;
    // need to calculate duration (checking first since seemed most likely to happen)
    if (startDate && endDate && !hasDuration) {
      me.setData('duration', DateHelper.diff(startDate, endDate, durationUnit, true));
    }
    // need to calculate endDate?
    else if (startDate && !endDate && hasDuration) {
      me.setData('endDate', DateHelper.add(startDate, duration, durationUnit));
    }
    // need to calculate startDate
    else if (!startDate && endDate && hasDuration) {
      me.setData('startDate', DateHelper.add(endDate, -duration, durationUnit));
    }
  }
  //endregion
  //region Getters & Setters
  updateInternalCls(cls) {
    if (this._cls) {
      this._cls.value = cls;
    } else {
      this._cls = new DomClassList(cls);
    }
  }
  set internalCls(cls) {
    this.updateInternalCls(cls);
    this.set('cls', this._cls.value);
  }
  get internalCls() {
    const {
      cls
    } = this;
    // `cls` getter can be overriden so return `cls` value if it is DomClassList or assign it to `this._cls`
    if (cls !== null && cls !== void 0 && cls.isDomClassList) {
      return cls;
    }
    this.internalCls = cls;
    return this._cls;
  }
  get cls() {
    if (!this._cls) {
      this._cls = new DomClassList(super.get('cls'));
    }
    return this._cls;
  }
  set cls(cls) {
    this.internalCls = cls;
  }
  get startDate() {
    return this.get('startDate');
  }
  set startDate(date) {
    this.setStartDate(date);
  }
  get endDate() {
    return this.get('endDate');
  }
  set endDate(date) {
    this.setEndDate(date);
  }
  get endingDate() {
    const me = this,
      {
        endDate,
        startDate
      } = me;
    if (endDate) {
      // Special case of startDate===endDate for allDay event:
      // if (Number(endDate) === Number(startDate) && me.allDay) {
      //     return DH.add(startDate, 1, 'd');
      // }
      // Nope... the above works fine except when the day start time is shifted. In this case we want the
      // event to appear as "all day" on the shifted day, but the above will push the endingDate beyond the
      // end of the shifted day.
      return endDate;
    }
    return DateHelper.add(startDate, me.duration, me.durationUnit);
  }
  get duration() {
    return this.get('duration');
  }
  set duration(duration) {
    this.setDuration(duration, this.durationUnit);
  }
  get durationUnit() {
    return this.get('durationUnit');
  }
  /**
   * Sets duration and durationUnit in one go. Only allowed way to change durationUnit, the durationUnit field is
   * readonly after creation
   * @param {Number} duration Duration value
   * @param {'millisecond'|'second'|'minute'|'hour'|'day'|'week'|'month'|'quarter'|'year'} durationUnit Unit for
   * specified duration value, see {@link #field-durationUnit} for valid values
   * @category Scheduling
   */
  setDuration(duration, durationUnit = this.durationUnit) {
    // Must be a number
    duration = parseFloat(duration);
    this.set({
      duration,
      durationUnit,
      ...this.updateDatesFromDuration(duration, durationUnit)
    });
  }
  updateDatesFromDuration(magnitude, unit, startDate = this.startDate, endDate = this.endDate) {
    const result = {};
    if (startDate) {
      result.endDate = DateHelper.add(startDate, magnitude, unit);
    } else if (endDate) {
      result.startDate = DateHelper.add(endDate, -magnitude, unit);
    }
    return result;
  }
  /**
   * Returns duration of the event in given unit. This is a wrapper for {@link Core.helper.DateHelper#function-getDurationInUnit-static}
   * @param {'ms'|'s'|'m'|'h'|'d'|'w'|'M'|'y'} unit
   * @param {Boolean} [doNotRound]
   * @private
   * @returns {Number}
   */
  getDurationInUnit(unit, doNotRound) {
    const me = this;
    if (me.startDate && me.endDate) {
      return DateHelper.getDurationInUnit(me.startDate, me.endDate, unit, doNotRound);
    } else {
      return DateHelper.as(unit, me.duration, me.durationUnit);
    }
  }
  get fullDuration() {
    // Used for formatting during export
    return new Duration({
      unit: this.durationUnit,
      magnitude: this.duration
    });
  }
  set fullDuration(duration) {
    if (typeof duration === 'string') {
      duration = DateHelper.parseDuration(duration, true, this.durationUnit);
    }
    this.setDuration(duration.magnitude, duration.unit);
  }
  /**
   * Sets the range start date
   *
   * @param {Date} date The new start date
   * @param {Boolean} keepDuration Pass `true` to keep the duration of the task ("move" the event), `false` to change the duration ("resize" the event).
   * Defaults to `true`
   * @category Scheduling
   */
  setStartDate(date, keepDuration = true) {
    const me = this,
      toSet = {
        startDate: date
      };
    if (date) {
      let calcEndDate;
      if (keepDuration) {
        calcEndDate = me.duration != null;
      } else {
        if (me.endDate) {
          toSet.duration = DateHelper.diff(date, me.endDate, me.durationUnit, true);
          if (toSet.duration < 0) throw new Error('Negative duration');
        } else {
          calcEndDate = this.duration != null;
        }
      }
      if (calcEndDate) {
        // Use hours to set end date in order to correctly process DST crossings
        toSet.endDate = DateHelper.add(date, me.getDurationInUnit('h'), 'h');
      }
    } else {
      toSet.duration = null;
    }
    me.set(toSet);
  }
  /**
   * Sets the range end date
   *
   * @param {Date} date The new end date
   * @param {Boolean} keepDuration Pass `true` to keep the duration of the task ("move" the event), `false` to change the duration ("resize" the event).
   * Defaults to `false`
   * @category Scheduling
   */
  setEndDate(date, keepDuration = false) {
    const me = this,
      toSet = {
        endDate: date
      };
    if (date) {
      let calcStartDate;
      if (keepDuration === true) {
        calcStartDate = me.duration != null;
      } else {
        if (me.startDate) {
          toSet.duration = DateHelper.diff(me.startDate, date, me.durationUnit, true);
          if (toSet.duration < 0) throw new Error('Negative duration');
        } else {
          calcStartDate = this.duration != null;
        }
      }
      if (calcStartDate) {
        toSet.startDate = DateHelper.add(date, -me.duration, me.durationUnit);
      }
    }
    me.set(toSet);
  }
  /**
   * Sets the event start and end dates
   *
   * @param {Date} start The new start date
   * @param {Date} end The new end date
   * @param {Boolean} [silent] Pass `true` to not trigger events
   * @category Scheduling
   */
  setStartEndDate(start, end, silent) {
    this.set({
      startDate: start,
      endDate: end
    }, null, silent);
  }
  /**
   * Returns an array of dates in this range. If the range starts/ends not at the beginning of day, the whole day will be included.
   * @readonly
   * @property {Date[]}
   * @category Scheduling
   */
  get dates() {
    const dates = [],
      startDate = DateHelper.startOf(this.startDate, 'day'),
      endDate = this.endDate;
    for (let date = startDate; date < endDate; date = DateHelper.add(date, 1, 'day')) {
      dates.push(date);
    }
    return dates;
  }
  get startDateMS() {
    var _ref;
    return (_ref = this.batching && this.hasBatchedChange('startDate') ? this.get('startDate') : this.startDate) === null || _ref === void 0 ? void 0 : _ref.getTime();
  }
  get endDateMS() {
    var _ref2;
    return (_ref2 = this.batching && this.hasBatchedChange('endDate') ? this.get('endDate') : this.endDate) === null || _ref2 === void 0 ? void 0 : _ref2.getTime();
  }
  /**
   * Returns the duration of this Event in milliseconds.
   * @readonly
   * @property {Number}
   * @category Scheduling
   */
  get durationMS() {
    const {
      endDateMS,
      startDateMS
    } = this;
    if (endDateMS && startDateMS) {
      return endDateMS - startDateMS;
    } else {
      return DateHelper.asMilliseconds(this.duration || 0, this.durationUnit);
    }
  }
  /**
   * Returns true if record is a milestone.
   * @readonly
   * @property {Boolean}
   * @category Scheduling
   */
  get isMilestone() {
    return this.duration === 0;
  }
  inSetNormalize(field) {
    if (typeof field !== 'string') {
      // If user is updating multiple properties in one go using an object, we help out
      // by filling out missing schedule related data
      let {
        startDate,
        endDate,
        duration,
        durationUnit = this.durationUnit
      } = field;
      // Conversion is usually handled in inSet, but we are normalizing prior to that and have to handle it here
      if (typeof startDate === 'string') {
        startDate = this.getFieldDefinition('startDate').convert(startDate);
      }
      if (typeof endDate === 'string') {
        endDate = this.getFieldDefinition('endDate').convert(endDate);
      }
      if ('duration' in field) {
        if (startDate && !endDate) {
          endDate = DateHelper.add(startDate, duration, durationUnit, true, true);
        }
        if (!startDate && endDate) {
          startDate = DateHelper.add(endDate, -duration, durationUnit, true, true);
        }
      } else if (startDate && endDate) {
        // Calculate duration in hours and covert to target duration unit in order to avoid extra DST conversion
        duration = DateHelper.as(durationUnit, DateHelper.diff(startDate, endDate, 'h', true), 'h');
      }
      // A framework (React tested) may freeze passed field object, so clone it in that case
      const fieldOrClone = Object.isFrozen(field) ? ObjectHelper.clone(field) : field;
      startDate && (fieldOrClone.startDate = startDate);
      endDate && (fieldOrClone.endDate = endDate);
      duration != null && (fieldOrClone.duration = duration);
      return fieldOrClone;
    }
  }
  fieldToKeys(field, value) {
    const result = super.fieldToKeys(field, value);
    // Replace fullDuration with duration and durationUnit in calls to `set()`
    if (result.fullDuration) {
      const {
        magnitude,
        unit
      } = result.fullDuration;
      result.duration = magnitude;
      result.durationUnit = unit;
    }
    // Engine handles this for event & tasks
    if (!this.isEventModel && !this.isTaskModel) {
      // Recalculate start/end date if duration is set with only one of them
      if (('duration' in result || result.durationUnit) && !(result.startDate && result.endDate)) {
        Object.assign(result, this.updateDatesFromDuration(result.duration ?? this.duration, result.durationUnit ?? this.durationUnit, result.startDate, result.endDate));
      }
    }
    return result;
  }
  inSet(field, value, silent, fromRelationUpdate, skipAccessors, validOnly) {
    if (!skipAccessors) {
      field = this.inSetNormalize(field) || field;
    }
    return super.inSet(field, value, silent, fromRelationUpdate, skipAccessors, validOnly);
  }
  // Cls requires special handling since it is converted to a DomClassList
  applyValue(useProp, key, value, skipAccessors, field) {
    if (key === 'cls') {
      this.updateInternalCls(value);
    }
    // When setting fullDuration with record.set()
    if (key === 'fullDuration') {
      this[key] = value;
    }
    super.applyValue(useProp, key, value, skipAccessors, field);
  }
  //endregion
  //region Iteration
  /**
   * Iterates over the {@link #property-dates}
   * @param {Function} func The function to call for each date
   * @param {Object} thisObj `this` reference for the function
   * @category Scheduling
   */
  forEachDate(func, thisObj) {
    return this.dates.forEach(func.bind(thisObj));
  }
  //endregion
  /**
   * Checks if the range record has both start and end dates set and start <= end
   *
   * @property {Boolean}
   * @category Scheduling
   */
  get isScheduled() {
    const {
      startDateMS,
      endDateMS
    } = this;
    return endDateMS - startDateMS >= 0;
  }
  // Simple check if end date is greater than start date
  get isValid() {
    const {
      startDate,
      endDate
    } = this;
    return !startDate || !endDate || endDate - startDate >= 0;
  }
  /**
   * Shift the dates for the date range by the passed amount and unit
   * @param {'ms'|'s'|'m'|'h'|'d'|'w'|'M'|'y'} unit The unit to shift by, see {@link Core.helper.DateHelper}
   * for more information on valid formats.
   * @param {Number} amount The amount to shift
   */
  shift(amount, unit = this.durationUnit) {
    if (typeof amount === 'string') {
      const u = amount;
      amount = unit;
      unit = u;
    }
    return this.setStartDate(DateHelper.add(this.startDate, amount, unit, true), true);
  }
  /**
   * Returns the WBS code of this model (e.g '2.1.3'). Only relevant when part of a tree store, as in the Gantt chart.
   * @property {String}
   * @category Parent & children
   */
  get wbsCode() {
    return this._wbsCode || this.indexPath.join('.');
  }
  set wbsCode(value) {
    // wbsCode needs to be writable to interop w/TaskModel and Baselines which copy this field value
    this._wbsCode = value;
  }
  fullCopy() {
    //NOT PORTED
    return this.copy.apply(this, arguments);
  }
  intersects(timeSpan) {
    return this.intersectsRange(timeSpan.startDate, timeSpan.endDate);
  }
  intersectsRange(start, end) {
    const myStart = this.startDate,
      myEnd = this.endDate;
    return myStart && myEnd && DateHelper.intersectSpans(myStart, myEnd, start, end);
  }
  /**
   * Splits this event into two pieces at the desired position.
   *
   * @param {Number|String} splitPoint The duration point at which to split this event.
   *
   * If less then `1`, this indicates the relative position at which it will be split.
   * 0.5 means cut it in half.
   *
   * If greater than `1`, this indicates the new duration in the current duration units of this event before the split.
   *
   * If this is a string, it will be a duration description as described in
   * {@link Core.helper.DateHelper#function-parseDuration-static}, for example `'15 min'`
   *
   * @returns {Scheduler.model.TimeSpan} The newly created split section of the timespan
   * @category Scheduling
   */
  split(splitPoint = 0.5) {
    const me = this,
      clone = me.copy(),
      {
        fullDuration,
        eventStore,
        assignmentStore
      } = me,
      oldDuration = new Duration(fullDuration),
      cloneDuration = new Duration(fullDuration);
    let ownNewDuration, unitsChanged;
    if (typeof splitPoint === 'string') {
      ownNewDuration = new Duration(splitPoint);
      // New duration specified in same time units as current duration
      if (ownNewDuration.unit === oldDuration.unit) {
        cloneDuration.magnitude -= ownNewDuration.magnitude;
      }
      // New duration is in different units, so convert clone's duration to match
      else {
        cloneDuration.magnitude = DateHelper.as(ownNewDuration.unit, oldDuration) - ownNewDuration.magnitude;
        cloneDuration.unit = ownNewDuration.unit;
        unitsChanged = true;
      }
    } else {
      ownNewDuration = new Duration(splitPoint > 1 ? splitPoint : me.duration * splitPoint, me.durationUnit);
      cloneDuration.magnitude -= ownNewDuration.magnitude;
    }
    clone.startDate = DateHelper.add(me.startDate, ownNewDuration.magnitude, ownNewDuration.unit);
    if (unitsChanged) {
      clone.fullDuration = cloneDuration;
      me.fullDuration = ownNewDuration;
    } else {
      clone.duration = cloneDuration.magnitude;
      me.duration = ownNewDuration.magnitude;
    }
    if (eventStore) {
      eventStore.add(clone);
      if (assignmentStore && !eventStore.usesSingleAssignment) {
        assignmentStore.add(me.assignments.map(assignment => {
          const clonedData = Object.assign({}, assignment.data, {
            eventId: clone.id,
            // From engine
            event: null,
            resource: null
          });
          delete clonedData.id;
          return clonedData;
        }));
      }
    }
    return clone;
  }
  toICSString(icsEventConfig = {}) {
    if (!this.isScheduled) {
      return '';
    }
    const {
        startDate,
        endDate
      } = this,
      // To allow testing using a fixed timestamp value
      timestamp = icsEventConfig.DTSTAMP || DateHelper.format(new Date(), 'uu');
    delete icsEventConfig.DTSTAMP;
    let startEnd = {};
    if (this.allDay) {
      startEnd = {
        'DTSTART;VALUE=DATE': DateHelper.format(startDate, 'u'),
        'DTEND;VALUE=DATE': DateHelper.format(endDate, 'u')
      };
    } else {
      startEnd = {
        DTSTART: DateHelper.format(startDate, 'uu'),
        DTEND: DateHelper.format(endDate, 'uu')
      };
    }
    const version = VersionHelper.scheduler && VersionHelper.getVersion('scheduler') || VersionHelper.calendar && VersionHelper.getVersion('calendar') || '',
      icsWrapConfig = {
        BEGIN: 'VCALENDAR',
        VERSION: '2.0',
        CALSCALE: 'GREGORIAN',
        PRODID: `-//Bryntum AB//Bryntum Scheduler ${version} //EN`,
        END: 'VCALENDAR'
      },
      eventConfig = {
        BEGIN: 'VEVENT',
        UID: this.id + '@bryntum.com',
        CLASS: 'PUBLIC',
        SUMMARY: this.name,
        DTSTAMP: timestamp,
        ...startEnd,
        ...(this.recurrenceRule ? {
          RRULE: this.recurrenceRule
        } : {}),
        ...icsEventConfig,
        END: 'VEVENT'
      },
      icsItems = Object.keys(icsWrapConfig).map(key => `${key}:${icsWrapConfig[key]}`),
      eventItems = Object.keys(eventConfig).map(key => `${key}:${eventConfig[key]}`);
    // Inject event details before the closing VCALENDAR entry
    icsItems.splice(icsItems.length - 1, 0, ...eventItems);
    return icsItems.join('\n');
  }
  /**
   * Triggers a download of this time span in ICS format (for import in Outlook etc.)
   *
   * ```javascript
   * timeSpan.downloadAsICS({
   *      LOCATION : timeSpan.location
   *  });
   * ```
   * @param {Object<String,String>} [icsEventConfig] A config object with properties to be added in to `BEGIN:VEVENT`
   * section of the exported event.
   * @category Misc
   */
  exportToICS(icsEventConfig) {
    if (this.isScheduled) {
      const blob = new Blob([this.toICSString(icsEventConfig)], {
        type: 'text/calendar'
      });
      BrowserHelper.downloadBlob(blob, (this.name || 'Event') + '.ics');
    }
  }
  /**
   * Defines if the given event field should be manually editable in UI.
   * You can override this method to provide your own logic.
   *
   * By default the method defines all the event fields as editable.
   *
   * @param {String} fieldName Name of the field
   * @returns {Boolean} Returns `true` if the field is editable, `false` if it is not and `undefined` if the model has no such field.
   */
  isEditable(fieldName) {
    // return undefined for unknown fields
    return this.getFieldDefinition(fieldName) ? true : undefined;
  }
  isFieldModified(fieldName) {
    if (fieldName === 'fullDuration') {
      return super.isFieldModified('duration') || super.isFieldModified('durationUnit');
    }
    return super.isFieldModified(fieldName);
  }
}
TimeSpan._$name = 'TimeSpan';

const dayParseRegExp = /^([+-]?[0-9])?(SU|MO|TU|WE|TH|FR|SA)$/;
const days = ['SU', 'MO', 'TU', 'WE', 'TH', 'FR', 'SA'];
class RecurrenceDayRuleEncoder extends Base {
  static decodeDay(rawDay) {
    let parsedDay, result;
    if (parsedDay = dayParseRegExp.exec(rawDay)) {
      result = [days.indexOf(parsedDay[2])];
      // optional position number
      if (parsedDay[1]) {
        parsedDay[1] = parseInt(parsedDay[1], 10);
        result.push(parsedDay[1]);
      }
    }
    return result;
  }
  static encodeDay(day) {
    let position;
    // support decodeDay() result format
    if (Array.isArray(day)) {
      [day, position] = day;
    }
    return (position ? position.toString() : '') + days[day];
  }
  // Turns days values provided as an array of strings (like [`-1MO`, `SU`, `+3FR`])
  // into an array of [ dayIndex, position ] elements, where:
  //
  // - `dayIndex` - zero-based week day index value (0 - Sunday, 1 - Monday, 2 - Tuesday, etc.)
  // - `position` - (optional) 1-based position of the day (integer value (can be both positive and negative))
  static decode(rawDays) {
    const result = [];
    let parsedDay;
    if (rawDays) {
      for (let i = 0; i < rawDays.length; i++) {
        if (parsedDay = this.decodeDay(rawDays[i])) {
          result.push(parsedDay);
        }
      }
    }
    return result;
  }
  static encode(days) {
    const result = [];
    let day;
    if (days) {
      for (let i = 0; i < days.length; i++) {
        if (day = this.encodeDay(days[i])) {
          result.push(day);
        }
      }
    }
    return result;
  }
}
RecurrenceDayRuleEncoder._$name = 'RecurrenceDayRuleEncoder';

/**
 * @module Scheduler/data/util/recurrence/AbstractRecurrenceIterator
 * @private
 */
const frequencyToUnitRe = /ly$/i,
  frequencyToUnit = frequency => {
    const result = frequency.replace(frequencyToUnitRe, '');
    return result === 'DAI' ? 'DAY' : result;
  },
  fn = (date, counter, isFirst, timeSpan) => timeSpan.buildOccurrence(date, isFirst),
  captureLastOccurrence = date => lastOccurrenceDate = date;
let lastOccurrenceDate;
class AbstractRecurrenceIterator extends Base {
  static frequency = 'NONE';
  static MAX_OCCURRENCES_COUNT = 1000000;
  static processIterationConfig(config) {
    var _endDate;
    const {
        recurrence
      } = config,
      {
        frequency,
        interval,
        timeSpan,
        endDate: until,
        count
      } = recurrence;
    // Force the correction of the event to be in sync with its recurrence rule
    // before performing iteration. For example, if the event's configured startDate
    // is 1st January 2020, and the rule dictates that the event will take place
    // monthly, every Monday, then the first event is not until Monday 6th January, 2020.
    if (!config.syncingStartDate && !timeSpan.meta.isSyncedWithRule) {
      // Do not generate occurrences outside of the specified range
      const intervalEndDate = DateHelper.add(timeSpan.startDate, interval, frequencyToUnit(frequency)),
        endDate = DateHelper.min(intervalEndDate, config.endDate || intervalEndDate);
      this.forEachDate({
        syncingStartDate: true,
        startDate: timeSpan.startDate,
        endDate,
        recurrence,
        fn
      });
    }
    // Capture the start after its been synced with its recurrence rule
    const timeSpanStart = timeSpan.startDate;
    // Extract the endDate from the config, defaulting to the recurrence UNTIL date
    let {
      startDate = timeSpanStart,
      endDate = until
    } = config;
    // No point in starting the iteration before the event starts
    if (startDate < timeSpanStart) {
      startDate = timeSpanStart;
    }
    // The recurrence's stop date overrides the configured endDate.
    if (until) {
      if (!endDate || endDate > until) {
        endDate = until;
      }
    }
    // If we are limiting using count and we are not starting from the
    // first occurrence, then we have to calculate a stop date.
    // This is because for date ranges in the future we cannot calculate how many
    // preceding occurrences there may have been.
    else if (count && startDate > timeSpanStart) {
      // Iterate the occurrences from the start to capture the last one
      this.forEachDate({
        recurrence,
        fn: captureLastOccurrence
      });
      // The date of the last occurrence in the count sequence overrides the configured endDate.
      if (!endDate || endDate > lastOccurrenceDate) {
        // since endDate of the provided interval is exclusive add 1 millisecond
        endDate = new Date(lastOccurrenceDate.getTime() + 1);
      }
    }
    // Preserve the requested start of requested visits.
    const earliestVisibleDate = startDate;
    // Unless we are only asked for events which *start* within the time range. we must make
    // a best attempt to include any occurrences which span the start date.
    // So if we are asking for events from the 1st of the month, and there's an event
    // which runs every 2 months from the 15th to the 5th, we must include it. Start the
    // iteration <interval> full frequency quanta before the requested start.
    // This will only cause <interval> extra iterations.
    // We cannot step back to before the event's starting date though.
    if (!config.startOnly) {
      startDate = new Date(DateHelper.max(DateHelper.add(startDate, -interval, frequencyToUnit(frequency)), timeSpanStart));
    }
    return Object.assign({
      extraArgs: [],
      // Only check start date for time spans with 0 duration
      startOnly: !Boolean(timeSpan.durationMS)
    }, config, {
      startDate,
      endDate,
      timeSpan,
      timeSpanStart,
      earliestVisibleDate,
      endDateMS: (_endDate = endDate) === null || _endDate === void 0 ? void 0 : _endDate.getTime(),
      timeSpanStartMS: timeSpanStart.getTime(),
      earliestVisibleDateMS: earliestVisibleDate.getTime(),
      durationMS: timeSpan.durationMS,
      spansStart: startDate <= timeSpanStart && endDate > timeSpanStart
    });
  }
  static getOccurrenceIndex(event) {
    if (event.isOccurrence) {
      return DateHelper.diff(event.recurringTimeSpan.startDate, event.startDate, frequencyToUnit(event.recurringTimeSpan.recurrence.frequency));
    }
  }
  /**
   * Returns Nth occurrence of a week day in the provided period of time.
   * @param  {Date} startDate Period start date.
   * @param  {Date} endDate   Period end date.
   * @param  {Number} day    Week day (0 - Sunday, 1 - Monday, 2 - Tuesday, etc.)
   * @param  {Number} index  Index to find.
   * @returns {Date}           Returns the found date or null if there is no `index`th entry.
   * @private
   */
  static getNthDayInPeriod(startDate, endDate, day, index) {
    let result, sign, borderDate;
    if (index) {
      if (index > 0) {
        sign = 1;
        borderDate = startDate;
      } else {
        sign = -1;
        borderDate = endDate;
      }
      // delta between requested day and border day
      const delta = day - borderDate.getDay();
      // if the requested day goes after (before, depending on borderDate used (start/end))
      // we adjust index +/-1
      if (sign * delta < 0) {
        index += sign;
      }
      // measure "index" weeks forward (or backward) ..take delta into account
      // result = new Date(borderDate.getTime() + ((index - sign) * 7 + delta) * 24*60*60*1000);
      // Per https://github.com/bryntum/support/issues/3413 - don't do that ^^ because DST makes days/week
      // alternatingly shorter or longer depending on going in/out of DST:
      result = new Date(borderDate);
      result.setDate(borderDate.getDate() + (index - sign) * 7 + delta);
      // if resulting date is outside of the provided range there is no "index"-th entry
      // of the day
      if (result < startDate || result > endDate) {
        result = null;
      }
    }
    return result;
  }
  static buildDate(year, month, date) {
    const dt = new Date(year, month, date);
    if (dt.getFullYear() === year && dt.getMonth() === month && dt.getDate() === date) {
      return dt;
    }
  }
  static isValidPosition(position) {
    return Boolean(position);
  }
  static forEachDateAtPositions(dates, positions, fn, scope) {
    const datesLength = dates.length,
      processed = {};
    for (let i = 0; i < positions.length; i++) {
      const index = positions[i];
      if (this.isValidPosition(index)) {
        const date = index > 0 ? dates[index - 1] : dates[datesLength + index];
        if (date && !processed[date.getTime()]) {
          // remember that we've returned the date
          processed[date.getTime()] = true;
          // return false if it's time to stop recurring
          if (fn.call(scope, date) === false) {
            return false;
          }
        }
      }
    }
  }
  static isInView(startOnly, occurrenceDate, earliestVisibleDate, durationMS, timeSpan) {
    return (startOnly ? occurrenceDate >= earliestVisibleDate : occurrenceDate.valueOf() + durationMS > earliestVisibleDate) && !timeSpan.hasException(occurrenceDate);
  }
  // Slightly faster version of ^, used by Daily & Weekly iterators
  static isInViewMS(startOnly, occurenceDate, occurrenceDateMS, earliestVisibleDateMS, durationMS, timeSpan) {
    return (startOnly ? occurrenceDateMS >= earliestVisibleDateMS : occurrenceDateMS + durationMS > earliestVisibleDateMS) && !timeSpan.hasException(occurenceDate);
  }
}
AbstractRecurrenceIterator._$name = 'AbstractRecurrenceIterator';

/**
 * @module Scheduler/data/util/recurrence/DailyRecurrenceIterator
 */
/**
 * A class which provides iteration to call a function for dates specified by a
 * {@link Scheduler.model.RecurrenceModel RecurrenceModel} over a specified date range.
 * @private
 */
class DailyRecurrenceIterator extends AbstractRecurrenceIterator {
  static frequency = 'DAILY';
  /**
   * Iterates over the passed date range, calling the passed callback on each date on which
   * starts a recurring event which matches the passed recurrence rule and overlaps the start and end dates
   * and is not an {@link Scheduler.model.mixin.RecurringTimeSpan#field-exceptionDates exceptionDate}
   * in the recurring event.
   * @param {Object} config An object which describes how to iterate.
   * @param {Date} config.startDate The point in time to begin iteration.
   * @param {Date} config.endDate The point in time to end iteration.
   * @param {Boolean} [config.startOnly] By default, all occurrences which intersect the date range
   * will be visited. Pass `true` to only visit occurrences which *start* in the date range.
   * @param {Scheduler.model.RecurrenceModel} config.recurrence The point in time to end iteration.
   * @param {Function} config.fn The function to call for each date which matches the recurrence in the date range.
   * @param {Date} config.fn.date The occurrence date.
   * @param {Number} config.fn.counter A counter of how many dates have been visited in this iteration.
   * @param {Boolean} config.fn.isFirst A flag which is `true` if the date is the first occurrence in the specified recurrence rule.
   * @param {Array} [config.extraArgs] Extra arguments to pass to the callback after the `isFirst` argument.
   */
  static forEachDate(config) {
    const {
        startOnly,
        startDate,
        endDate,
        endDateMS,
        timeSpan,
        timeSpanStart,
        earliestVisibleDateMS,
        durationMS,
        spansStart,
        recurrence,
        fn,
        extraArgs,
        scope = this
      } = this.processIterationConfig(config),
      {
        interval
      } = recurrence,
      delay = startDate - timeSpanStart,
      // recurrence interval duration in ms (86400000 is a single day duration in ms)
      intervalDuration = interval * 86400000,
      delayInIntervals = Math.floor(delay / intervalDuration);
    let {
        count
      } = recurrence,
      counter = 0,
      occurrenceDate = DateHelper.add(timeSpanStart, delayInIntervals * interval, 'day'),
      occurrenceDateMS = occurrenceDate.getTime();
    if (!endDate && !count) {
      count = this.MAX_OCCURRENCES_COUNT;
    }
    while (!endDateMS || occurrenceDateMS < endDateMS) {
      const inView = this.isInViewMS(startOnly, occurrenceDate, occurrenceDateMS, earliestVisibleDateMS, durationMS, timeSpan);
      counter++;
      if (inView && (endDateMS && occurrenceDateMS > endDateMS || fn.apply(scope, [occurrenceDate, counter, counter === 1 && spansStart, timeSpan, ...extraArgs]) === false || count && counter >= count)) {
        break;
      }
      // shift to the next day
      occurrenceDate = DateHelper.add(occurrenceDate, interval, 'day');
      occurrenceDateMS = occurrenceDate.getTime();
    }
  }
}
DailyRecurrenceIterator._$name = 'DailyRecurrenceIterator';

/**
 * @module Scheduler/data/util/recurrence/WeeklyRecurrenceIterator
 */
/**
 * A class which provides iteration to call a function for dates specified by a
 * {@link Scheduler.model.RecurrenceModel RecurrenceModel} over a specified date range.
 * @private
 */
class WeeklyRecurrenceIterator extends AbstractRecurrenceIterator {
  static frequency = 'WEEKLY';
  /**
   * Iterates over the passed date range, calling the passed callback on each date on which
   * starts an event which matches the passed recurrence rule and overlaps the start and end dates.
   * @param {Object} config An object which describes how to iterate.
   * @param {Date} config.startDate The point in time to begin iteration.
   * @param {Date} config.endDate The point in time to end iteration.
   * @param {Boolean} [config.startOnly] By default, all occurrences which intersect the date range
   * will be visited. Pass `true` to only visit occurrences which *start* in the date range.
   * @param {Scheduler.model.RecurrenceModel} config.recurrence The point in time to end iteration.
   * @param {Function} config.fn The function to call for each date which matches the recurrence in the date range.
   * @param {Date} config.fn.date The occurrence date.
   * @param {Number} config.fn.counter A counter of how many dates have been visited in this iteration.
   * @param {Boolean} config.fn.isFirst A flag which is `true` if the date is the first occurrence in the specified recurrence rule.
   * @param {Array} [config.extraArgs] Extra arguments to pass to the callback after the `isFirst` argument.
   */
  static forEachDate(config) {
    var _weekDays;
    const {
        startOnly,
        startDate,
        endDateMS,
        timeSpan,
        timeSpanStart,
        timeSpanStartMS,
        earliestVisibleDateMS,
        durationMS,
        spansStart,
        recurrence,
        fn,
        extraArgs,
        scope = this
      } = this.processIterationConfig(config),
      {
        interval,
        days
      } = recurrence,
      {
        weekStartDay
      } = DateHelper,
      startHours = timeSpanStart.getHours(),
      startMinutes = timeSpanStart.getMinutes(),
      startSeconds = timeSpanStart.getSeconds(),
      startMS = timeSpanStart.getMilliseconds();
    let counter = 0,
      {
        count
      } = recurrence,
      weekDays = RecurrenceDayRuleEncoder.decode(days),
      weekStartDate,
      occurrenceDate;
    // "Days" might be skipped then we use the event start day
    if (!((_weekDays = weekDays) !== null && _weekDays !== void 0 && _weekDays.length)) {
      weekDays = [[timeSpanStart.getDay()]];
    }
    // If week start day is not zero (Sunday)
    // we need to normalize weekDays array since its values are used
    // to calculate real dates as: date = week_start_date + weekDay_entry
    // which does not work when week starts on non-Sunday
    if (weekStartDay > 0) {
      for (let i = 0; i < weekDays.length; i++) {
        if (weekStartDay > weekDays[i][0]) {
          weekDays[i][0] = 7 - weekStartDay - weekDays[i][0];
        } else {
          weekDays[i][0] -= weekStartDay;
        }
      }
    }
    // days could be provided in any order so it's important to sort them
    weekDays.sort((a, b) => a[0] - b[0]);
    // if the recurrence is limited w/ "Count" or not every interval should match
    // we need to 1st count passed occurrences so we always start iteration from the event start date
    weekStartDate = DateHelper.getNext(count || interval > 1 ? timeSpanStart : startDate, 'week', 0);
    if (!endDateMS && !count) {
      count = this.MAX_OCCURRENCES_COUNT;
    }
    while (!endDateMS || weekStartDate.getTime() < endDateMS) {
      for (let i = 0; i < weekDays.length; i++) {
        // Faster than chaining multiple DateHelper calls
        occurrenceDate = new Date(weekStartDate.getFullYear(), weekStartDate.getMonth(), weekStartDate.getDate() + weekDays[i][0], startHours, startMinutes, startSeconds, startMS);
        const occurrenceDateMS = occurrenceDate.getTime();
        if (occurrenceDateMS >= timeSpanStartMS) {
          const inView = this.isInViewMS(startOnly, occurrenceDate, occurrenceDateMS, earliestVisibleDateMS, durationMS, timeSpan);
          counter++;
          if (inView && (endDateMS && occurrenceDateMS >= endDateMS || fn.apply(scope, [occurrenceDate, counter, counter === 1 && spansStart, timeSpan, ...extraArgs]) === false || count && counter >= count)) {
            return;
          }
        }
      }
      // get next week start
      weekStartDate = DateHelper.getNext(weekStartDate, 'week', interval);
    }
  }
}
WeeklyRecurrenceIterator._$name = 'WeeklyRecurrenceIterator';

/**
 * @module Scheduler/data/util/recurrence/MonthlyRecurrenceIterator
 */
/**
 * A class which provides iteration to call a function for dates specified by a
 * {@link Scheduler.model.RecurrenceModel RecurrenceModel} over a specified date range.
 * @private
 */
class MonthlyRecurrenceIterator extends AbstractRecurrenceIterator {
  static frequency = 'MONTHLY';
  static getNthDayOfMonth(date, dayNum) {
    const daysInMonth = DateHelper.daysInMonth(date);
    let result = null;
    if (dayNum && Math.abs(dayNum) <= daysInMonth) {
      result = new Date(date.getFullYear(), date.getMonth(), dayNum < 0 ? daysInMonth + dayNum + 1 : dayNum);
    }
    return result;
  }
  static isValidPosition(position) {
    return position && Math.abs(position) > 0 && Math.abs(position) <= 31;
  }
  /**
   * Iterates over the passed date range, calling the passed callback on each date on which
   * starts an event which matches the passed recurrence rule and overlaps the start and end dates.
   * @param {Object} config An object which describes how to iterate.
   * @param {Date} config.startDate The point in time to begin iteration.
   * @param {Date} config.endDate The point in time to end iteration.
   * @param {Boolean} [config.startOnly] By default, all occurrences which intersect the date range
   * will be visited. Pass `true` to only visit occurrences which *start* in the date range.
   * @param {Scheduler.model.RecurrenceModel} config.recurrence The point in time to end iteration.
   * @param {Function} config.fn The function to call for each date which matches the recurrence in the date range.
   * @param {Date} config.fn.date The occurrence date.
   * @param {Number} config.fn.counter A counter of how many dates have been visited in this iteration.
   * @param {Boolean} config.fn.isFirst A flag which is `true` if the date is the first occurrence in the specified recurrence rule.
   * @param {Array} [config.extraArgs] Extra arguments to pass to the callback after the `isFirst` argument.
   */
  static forEachDate(config) {
    const {
        startOnly,
        startDate,
        endDate,
        timeSpan,
        timeSpanStart,
        earliestVisibleDate,
        durationMS,
        spansStart,
        recurrence,
        fn,
        extraArgs,
        scope = this
      } = this.processIterationConfig(config),
      {
        interval,
        days,
        count,
        positions
      } = recurrence,
      weekDays = RecurrenceDayRuleEncoder.decode(days),
      hasPositions = positions && positions.length,
      processedDate = {};
    let {
        monthDays
      } = recurrence,
      counter = 0,
      weekDayPosition,
      monthStartDate,
      monthEndDate,
      dates,
      occurrenceDate,
      i;
    // if the recurrence is limited w/ "Count" or not every interval should match
    // we need to 1st count passed occurrences so we always start iteration from the event start date
    monthStartDate = DateHelper.startOf(count || interval > 1 ? timeSpanStart : startDate, 'month');
    monthEndDate = new Date(DateHelper.getNext(monthStartDate, 'month', 1) - 1);
    // If no month days nor week days are provided let's use event start date month day
    if (!(monthDays && monthDays.length) && !(weekDays && weekDays.length)) {
      monthDays = [timeSpanStart.getDate()];
    }
    if (weekDays && weekDays.length) {
      // Collect hash of positions indexed by week days
      weekDays.forEach(day => {
        if (day[1]) {
          weekDayPosition = weekDayPosition || {};
          weekDayPosition[day[0]] = day[1];
        }
      });
    }
    while ((!endDate || endDate > monthStartDate) && (!count || counter < count)) {
      dates = [];
      if (weekDays && weekDays.length) {
        weekDays.forEach(day => {
          const weekDay = day[0];
          let from = 1,
            till = 53;
          // if position provided
          if (day[1]) {
            from = till = day[1];
          }
          for (i = from; i <= till; i++) {
            if (occurrenceDate = this.getNthDayInPeriod(monthStartDate, monthEndDate, weekDay, i)) {
              occurrenceDate = DateHelper.copyTimeValues(occurrenceDate, timeSpanStart);
              if (!processedDate[occurrenceDate.getTime()]) {
                // remember we processed the date
                processedDate[occurrenceDate.getTime()] = true;
                dates.push(occurrenceDate);
              }
            }
          }
        });
        dates.sort((a, b) => a - b);
        if (!hasPositions) {
          for (i = 0; i < dates.length; i++) {
            occurrenceDate = dates[i];
            if (occurrenceDate >= timeSpanStart) {
              const inView = this.isInView(startOnly, occurrenceDate, earliestVisibleDate, durationMS, timeSpan);
              counter++;
              if (inView && (endDate && occurrenceDate >= endDate || fn.apply(scope, [occurrenceDate, counter, counter === 1 && spansStart, timeSpan, ...extraArgs]) === false || count && counter >= count)) {
                return false;
              }
            }
          }
        }
      } else {
        const sortedMonthDates = [];
        for (i = 0; i < monthDays.length; i++) {
          // check if the date wasn't iterated over yet
          if ((occurrenceDate = this.getNthDayOfMonth(monthStartDate, monthDays[i])) && !processedDate[occurrenceDate.getTime()]) {
            processedDate[occurrenceDate.getTime()] = true;
            sortedMonthDates.push(occurrenceDate);
          }
        }
        // it's important to sort the dates to iterate over them in the proper order
        sortedMonthDates.sort((a, b) => a - b);
        for (i = 0; i < sortedMonthDates.length; i++) {
          occurrenceDate = DateHelper.copyTimeValues(sortedMonthDates[i], timeSpanStart);
          if (hasPositions) {
            dates.push(occurrenceDate);
          } else if (occurrenceDate >= timeSpanStart) {
            const inView = this.isInView(startOnly, occurrenceDate, earliestVisibleDate, durationMS, timeSpan);
            counter++;
            if (inView && (endDate && occurrenceDate > endDate || fn.apply(scope, [occurrenceDate, counter, counter === 1 && spansStart, timeSpan, ...extraArgs]) === false || count && counter >= count)) {
              return;
            }
          }
        }
      }
      if (hasPositions && dates.length) {
        this.forEachDateAtPositions(dates, positions, occurrenceDate => {
          if (occurrenceDate >= timeSpanStart) {
            const inView = startOnly ? occurrenceDate >= earliestVisibleDate : occurrenceDate.valueOf() + durationMS > earliestVisibleDate && !timeSpan.hasException(occurrenceDate);
            counter++;
            // Ignore dates outside of the [startDate, endDate] range
            if (inView && (!endDate || occurrenceDate <= endDate) && (
            // return false if it's time to stop recurring
            fn.apply(scope, [occurrenceDate, counter, counter === 1 && spansStart, timeSpan, ...extraArgs]) === false || count && counter >= count)) {
              return false;
            }
          }
        });
      }
      // get next month start
      monthStartDate = DateHelper.getNext(monthStartDate, 'month', interval);
      monthEndDate = new Date(DateHelper.getNext(monthStartDate, 'month', 1) - 1);
    }
  }
}
MonthlyRecurrenceIterator._$name = 'MonthlyRecurrenceIterator';

/**
 * @module Scheduler/data/util/recurrence/YearlyRecurrenceIterator
 */
/**
 * A class which provides iteration to call a function for dates specified by a
 * {@link Scheduler.model.RecurrenceModel RecurrenceModel} over a specified date range.
 * @private
 */
class YearlyRecurrenceIterator extends AbstractRecurrenceIterator {
  static frequency = 'YEARLY';
  /**
   * Iterates over the passed date range, calling the passed callback on each date on which
   * starts an event which matches the passed recurrence rule and overlaps the start and end dates.
   * @param {Object} config An object which describes how to iterate.
   * @param {Date} config.startDate The point in time to begin iteration.
   * @param {Date} config.endDate The point in time to end iteration.
   * @param {Boolean} [config.startOnly] By default, all occurrences which intersect the date range
   * will be visited. Pass `true` to only visit occurrences which *start* in the date range.
   * @param {Scheduler.model.RecurrenceModel} config.recurrence The point in time to end iteration.
   * @param {Function} config.fn The function to call for each date which matches the recurrence in the date range.
   * @param {Date} config.fn.date The occurrence date.
   * @param {Number} config.fn.counter A counter of how many dates have been visited in this iteration.
   * @param {Boolean} config.fn.isFirst A flag which is `true` if the date is the first occurrence in the specified recurrence rule.
   * @param {Array} [config.extraArgs] Extra arguments to pass to the callback after the `isFirst` argument.
   */
  static forEachDate(config) {
    const {
        startOnly,
        startDate,
        endDate,
        timeSpan,
        timeSpanStart,
        earliestVisibleDate,
        durationMS,
        spansStart,
        recurrence,
        fn,
        extraArgs,
        scope = this
      } = this.processIterationConfig(config),
      {
        interval,
        days,
        count,
        positions
      } = recurrence,
      weekDays = RecurrenceDayRuleEncoder.decode(days),
      hasPositions = positions && positions.length,
      processedDate = {};
    let {
        months
      } = recurrence,
      counter = 0,
      i,
      occurrenceDate,
      dates,
      yearStartDate,
      yearEndDate,
      weekDayPosition;
    // if the recurrence is limited w/ "Count" or not every interval should match
    // we need to 1st count passed occurrences so we always start iteration from the event start date
    yearStartDate = DateHelper.startOf(count || interval > 1 ? timeSpanStart : startDate, 'year');
    yearEndDate = new Date(DateHelper.getNext(yearStartDate, 'year', 1) - 1);
    months && months.sort((a, b) => a - b);
    // if no months provided let's use the event month
    if (!(months && months.length) && !(weekDays && weekDays.length)) {
      months = [timeSpanStart.getMonth() + 1];
    }
    if (weekDays && weekDays.length) {
      // Collect hash of positions indexed by week days
      weekDays.forEach(day => {
        if (day[1]) {
          weekDayPosition = weekDayPosition || {};
          weekDayPosition[day[0]] = day[1];
        }
      });
    }
    while ((!endDate || endDate > yearStartDate) && (!count || counter < count)) {
      dates = [];
      if (weekDays && weekDays.length) {
        weekDays.forEach(day => {
          const weekDay = day[0];
          let from = 1,
            till = 53;
          // if position provided
          if (day[1]) {
            from = till = day[1];
          }
          for (i = from; i <= till; i++) {
            if (occurrenceDate = this.getNthDayInPeriod(yearStartDate, yearEndDate, weekDay, i)) {
              occurrenceDate = DateHelper.copyTimeValues(occurrenceDate, timeSpanStart);
              if (!processedDate[occurrenceDate.getTime()]) {
                // remember we processed the date
                processedDate[occurrenceDate.getTime()] = true;
                dates.push(occurrenceDate);
              }
            }
          }
        });
        dates.sort((a, b) => a - b);
        if (!hasPositions) {
          for (i = 0; i < dates.length; i++) {
            occurrenceDate = dates[i];
            if (occurrenceDate >= timeSpanStart) {
              const inView = this.isInView(startOnly, occurrenceDate, earliestVisibleDate, durationMS, timeSpan);
              counter++;
              if (inView && (endDate && occurrenceDate >= endDate || fn.apply(scope, [occurrenceDate, counter, counter === 1 && spansStart, timeSpan, ...extraArgs]) === false || count && counter >= count)) {
                return;
              }
            }
          }
        }
      } else {
        for (i = 0; i < months.length; i++) {
          if (occurrenceDate = this.buildDate(yearStartDate.getFullYear(), months[i] - 1, timeSpanStart.getDate())) {
            occurrenceDate = DateHelper.copyTimeValues(occurrenceDate, timeSpanStart);
            // check if the date wasn't iterated over yet
            if (!processedDate[occurrenceDate.getTime()]) {
              processedDate[occurrenceDate.getTime()] = true;
              if (hasPositions) {
                dates.push(occurrenceDate);
              } else if (occurrenceDate >= timeSpanStart) {
                const inView = startOnly ? occurrenceDate >= earliestVisibleDate : occurrenceDate.valueOf() + durationMS > earliestVisibleDate && !timeSpan.hasException(occurrenceDate);
                counter++;
                if (inView && (endDate && occurrenceDate >= endDate || fn.apply(scope, [occurrenceDate, counter, counter === 1 && spansStart, timeSpan, ...extraArgs]) === false || count && counter >= count)) {
                  return;
                }
              }
            }
          }
        }
      }
      if (hasPositions && dates.length) {
        this.forEachDateAtPositions(dates, positions, occurrenceDate => {
          if (occurrenceDate >= timeSpanStart) {
            const inView = startOnly ? occurrenceDate >= earliestVisibleDate : occurrenceDate.valueOf() + durationMS > earliestVisibleDate && !timeSpan.hasException(occurrenceDate);
            counter++;
            // Ignore dates outside of the [startDate, endDate] range
            if (inView && (!endDate || occurrenceDate < endDate)) {
              // return false if it's time to stop recurring
              if (fn.apply(scope, [occurrenceDate, counter, counter === 1 && spansStart, timeSpan, ...extraArgs]) === false || count && counter >= count) {
                return false;
              }
            }
          }
        });
      }
      // get next month start
      yearStartDate = DateHelper.getNext(yearStartDate, 'year', interval);
      yearEndDate = new Date(DateHelper.getNext(yearStartDate, 'year', 1) - 1);
    }
  }
}
YearlyRecurrenceIterator._$name = 'YearlyRecurrenceIterator';

/**
 * @module Scheduler/model/RecurrenceModel
 */
const recurrenceIterators = {};
[DailyRecurrenceIterator, WeeklyRecurrenceIterator, MonthlyRecurrenceIterator, YearlyRecurrenceIterator].forEach(it => {
  recurrenceIterators[it.frequency] = it;
});
function convertStringOfIntegerItemsValue(value) {
  if (value) {
    if (typeof value == 'string') {
      value = value.split(',').map(item => parseInt(item, 10));
    }
  } else {
    value = null;
  }
  return value;
}
function convertStringOfItemsValue(value) {
  if (value) {
    if (typeof value == 'string') {
      value = value.split(',');
    }
  } else {
    value = null;
  }
  return value;
}
function isEqualAsString(value1, value2) {
  return String(value1) === String(value2);
}
function convertInteger(value) {
  if (this.defaultValue && value === undefined) {
    return this.defaultValue;
  }
  if (this.allowNull && value == null) {
    return null;
  }
  value = parseInt(value);
  return isNaN(value) ? undefined : value;
}
/**
 * This class represents a timespan recurrence settings.
 * It is a subclass of {@link Core.data.Model} class.
 * Please refer to the documentation for that class to become familiar with the base interface of this class.
 *
 * The data source for these fields can be customized by subclassing this class.
 *
 * @extends Core/data/Model
 */
class RecurrenceModel extends Model {
  static get $name() {
    return 'RecurrenceModel';
  }
  /**
   * Indicates that this is a `RecurrenceModel` class instance
   * (allows to avoid using `instanceof`).
   * @property {Boolean}
   * @readonly
   */
  get isRecurrenceModel() {
    return true;
  }
  //region Fields
  static get fields() {
    return [
    /**
     * Field defines the recurrence frequency. Supported values are: `DAILY`, `WEEKLY`, `MONTHLY`, `YEARLY`.
     * @field {'DAILY'|'WEEKLY'|'MONTHLY'|'YEARLY'} frequency
     */
    {
      name: 'frequency',
      defaultValue: 'DAILY'
    },
    /**
     * Field defines how often the recurrence repeats.
     * For example, if the recurrence is weekly its interval is 2, then the timespan repeats every two weeks.
     * @field {Number} interval
     */
    {
      name: 'interval',
      defaultValue: 1,
      convert: convertInteger
    },
    /**
     * End date of the recurrence. Specifies when the recurrence ends.
     * The value is optional, the recurrence can as well be stopped using {@link #field-count} field value.
     * @field {Date} endDate
     */
    {
      name: 'endDate',
      type: 'date'
    },
    /**
     * Specifies the number of occurrences after which the recurrence ends.
     * The value includes the associated timespan itself so values less than 2 make no sense.
     * The field is optional, the recurrence as well can be stopped using {@link #field-endDate} field value.
     * @field {Number} count
     */
    {
      name: 'count',
      allowNull: true,
      convert: convertInteger
    },
    /**
     * Specifies days of the week on which the timespan should occur.
     * An array of string values `SU`, `MO`, `TU`, `WE`, `TH`, `FR`, `SA`
     * corresponding to Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, and Saturday days of the week.
     * Each value can also be preceded by a positive (+n) or negative (-n) integer.
     * If present, this indicates the nth occurrence of a specific day within the monthly or yearly recurrence.
     *
     * **Not applicable** for daily {@link #field-frequency}.
     * @field {String[]} days
     */
    {
      name: 'days',
      convert: convertStringOfItemsValue,
      isEqual: isEqualAsString
    },
    /**
     * Specifies days of the month on which the timespan should occur.
     * An array of integer values (-31..-1 - +1..+31, negative values mean counting backwards from the month end).
     * **Applicable only** for monthly {@link #field-frequency}.
     * @field {Number[]} monthDays
     */
    {
      name: 'monthDays',
      convert: convertStringOfIntegerItemsValue,
      isEqual: isEqualAsString
    },
    /**
     * Specifies months of the year on which the timespan should occur.
     * An array of integer values (1 - 12).
     * **Applicable only** for yearly {@link #field-frequency}.
     * @field {Number[]} months
     */
    {
      name: 'months',
      convert: convertStringOfIntegerItemsValue,
      isEqual: isEqualAsString
    },
    /**
     * The positions to include in the recurrence. The values operate on a set of recurrence instances **in one interval** of the recurrence rule.
     * An array of integer values (valid values are 1 to 366 or -366 to -1, negative values mean counting backwards from the end of the built list of occurrences).
     * **Not applicable** for daily {@link #field-frequency}.
     * @field {Number} positions
     */
    {
      name: 'positions',
      convert: convertStringOfIntegerItemsValue,
      isEqual: isEqualAsString
    }];
  }
  get dateFormat() {
    return this._dateFormat || 'YYYYMMDDTHHmmss';
  }
  set dateFormat(format) {
    this._dateFormat = format;
  }
  get recurrenceIterator() {
    return recurrenceIterators[this.frequency];
  }
  /**
   * The timespan this recurrence is associated with.
   * @property {Scheduler.model.TimeSpan}
   */
  get timeSpan() {
    return this._timeSpan;
  }
  set timeSpan(value) {
    this._timeSpan = value;
  }
  /**
   * The recurrence rule. A string in [RFC-5545](https://tools.ietf.org/html/rfc5545#section-3.3.10) described format
   * ("RRULE" expression).
   * @property {String}
   */
  get rule() {
    const me = this,
      result = [];
    if (me.frequency) {
      result.push(`FREQ=${me.frequency}`);
      if (me.interval > 1) {
        result.push(`INTERVAL=${me.interval}`);
      }
      if (me.days && me.days.length) {
        result.push('BYDAY=' + me.days.join(','));
      }
      if (me.monthDays && me.monthDays.length) {
        result.push('BYMONTHDAY=' + me.monthDays.join(','));
      }
      if (me.months && me.months.length) {
        result.push('BYMONTH=' + me.months.join(','));
      }
      if (me.count) {
        result.push(`COUNT=${me.count}`);
      }
      if (me.endDate) {
        result.push('UNTIL=' + DateHelper.format(me.endDate, me.dateFormat));
      }
      if (me.positions && me.positions.length) {
        result.push('BYSETPOS=' + me.positions.join(','));
      }
    }
    return result.join(';');
  }
  set rule(rule) {
    const me = this,
      values = {
        frequency: null,
        interval: null,
        count: null,
        endDate: null,
        days: null,
        monthDays: null,
        months: null,
        positions: null
      };
    me.beginBatch();
    if (rule) {
      const parts = rule.split(';');
      for (let i = 0, len = parts.length; i < len; i++) {
        const part = parts[i].split('='),
          value = part[1];
        switch (part[0]) {
          case 'FREQ':
            values.frequency = value;
            break;
          case 'INTERVAL':
            values.interval = value;
            break;
          case 'COUNT':
            values.count = value;
            values.until = null;
            break;
          case 'UNTIL':
            if (value) {
              values.endDate = DateHelper.parse(value, me.dateFormat);
            } else {
              values.endDate = null;
            }
            values.count = null;
            break;
          case 'BYDAY':
            values.days = value;
            break;
          case 'BYMONTHDAY':
            values.monthDays = value;
            break;
          case 'BYMONTH':
            values.months = value;
            break;
          case 'BYSETPOS':
            values.positions = value;
            break;
        }
      }
    }
    me.set(values);
    if (rule) {
      me.sanitize();
    }
    me.endBatch();
  }
  construct(data = {}) {
    const me = this,
      {
        rule,
        timeSpan
      } = data;
    me._suspendedTimeSpanNotifying = 0;
    delete data.timeSpan;
    delete data.rule;
    super.construct(...arguments);
    if (rule) {
      me.suspendTimeSpanNotifying();
      me.rule = rule;
      me.resumeTimeSpanNotifying();
    }
    me.timeSpan = timeSpan;
  }
  /**
   * Iterate occurrences for the owning timespan across the specified date range. This method can be called even
   * if the timespan is not yet a member of a store, however, the occurrences returned will not be cached across
   * subsequent calls to this method.
   * @param {Date} startDate The start date of the iteration.
   * @param {Date} endDate The end date of the iteration.
   * @param {Function} fn The function to call for each occurrence.
   * @param {Scheduler.model.TimeSpan} fn.occurrence The occurrence.
   * @param {Boolean} fn.first A flag which is `true` for the first occurrence of this recurrence.
   * @param {Number} fn.counter A counter of how many dates have been visited in this iteration.
   * @param {Date} fn.date The occurrence date.
   * @internal
   */
  forEachOccurrence(startDate, endDate, fn) {
    if (this.timeSpan.startDate) {
      this.recurrenceIterator.forEachDate({
        recurrence: this,
        startDate,
        endDate,
        fn(date, counter, first, timeSpan) {
          return fn(timeSpan.buildOccurrence(date, first), first, counter, date);
        }
      });
    }
  }
  /**
   * Cleans up fields that do not makes sense for the current {@link #field-frequency} value.
   * @private
   */
  sanitize() {
    var _me$timeSpan;
    const me = this,
      timeSpanStartDate = (_me$timeSpan = me.timeSpan) === null || _me$timeSpan === void 0 ? void 0 : _me$timeSpan.startDate,
      values = {};
    me.isSanitizing = true;
    switch (me.frequency) {
      case 'DAILY':
        values.positions = null;
        values.days = null;
        values.monthDays = null;
        values.months = null;
        break;
      case 'WEEKLY':
        {
          values.positions = null;
          values.monthDays = null;
          values.months = null;
          const {
            days
          } = me;
          if (timeSpanStartDate && (days === null || days === void 0 ? void 0 : days.length) === 1 && days[0] === RecurrenceDayRuleEncoder.encodeDay(timeSpanStartDate.getDay())) {
            values.days = null;
          }
          break;
        }
      case 'MONTHLY':
        {
          var _me$monthDays;
          if ((_me$monthDays = me.monthDays) !== null && _me$monthDays !== void 0 && _me$monthDays.length) {
            values.positions = null;
            values.days = null;
          }
          values.months = null;
          const {
            monthDays
          } = me;
          if (timeSpanStartDate && (monthDays === null || monthDays === void 0 ? void 0 : monthDays.length) === 1 && monthDays[0] === timeSpanStartDate.getDate()) {
            values.monthDays = null;
          }
          break;
        }
      case 'YEARLY':
        {
          values.monthDays = null;
          const {
            months
          } = me;
          if (timeSpanStartDate && (months === null || months === void 0 ? void 0 : months.length) === 1 && months[0] === timeSpanStartDate.getMonth() + 1) {
            values.months = null;
          }
          break;
        }
    }
    me.set(values);
    me.isSanitizing = false;
  }
  copy(...args) {
    const result = super.copy(...args);
    result.dateFormat = this.dateFormat;
    result.timeSpan = this.timeSpan;
    return result;
  }
  afterChange(toSet, wasSet, silent) {
    const result = super.afterChange(toSet, wasSet, silent),
      {
        timeSpan
      } = this;
    if (!this.isSanitizing) {
      // cleanup data to match the chosen frequency
      this.sanitize();
    }
    if (timeSpan) {
      timeSpan.sanitizeRecurrenceData(this);
      if (!this.isTimeSpanNotifyingSuspended) {
        timeSpan.onRecurrenceChanged();
      }
    }
    return result;
  }
  set(field, value, ...args) {
    const values = typeof field === 'object' ? field : {
      [field]: value
    };
    // reset "endDate" field if "count" is being set
    if (values.count) {
      values.endDate = null;
    }
    // reset "count" field if "endDate" is being set
    else if (values.endDate) {
      values.count = null;
    }
    super.set(values, undefined, ...args);
  }
  get isTimeSpanNotifyingSuspended() {
    return Boolean(this._suspendedTimeSpanNotifying);
  }
  suspendTimeSpanNotifying() {
    this._suspendedTimeSpanNotifying++;
  }
  resumeTimeSpanNotifying() {
    if (this._suspendedTimeSpanNotifying) this._suspendedTimeSpanNotifying--;
  }
}
RecurrenceModel._$name = 'RecurrenceModel';

function convertExceptionDatesValue(value) {
  const result = {},
    {
      dateFormat
    } = this;
  if (value) {
    value = typeof value == 'string' ? value.split(',') : ArrayHelper.asArray(value);
    value.forEach(item => {
      if (typeof item == 'string') {
        item = DateHelper.parse(item, dateFormat);
      }
      // If we've got a valid date out of the incoming item, add an exception key
      if (!isNaN(item)) {
        result[DateHelper.makeKey(item)] = 1;
      }
    });
  }
  return result;
}
function serializeExceptionDatesValue(value) {
  const result = [],
    {
      dateFormat
    } = this;
  for (const date in value) {
    if (value[date]) {
      result.push(DateHelper.format(DateHelper.parseKey(date), dateFormat));
    }
  }
  return result;
}
const emptyArray = [];
/**
 * @module Scheduler/model/mixin/RecurringTimeSpan
 */
/**
 * This mixin class provides recurrence related fields and methods to a {@link Scheduler.model.TimeSpan timespan model}.
 *
 * The mixin introduces two types of timespans: __recurring timespan__ and its __occurrences__.
 * __Recurring timespan__ is a timespan having {@link #field-recurrenceRule recurrence rule} specified and its __occurrences__ are "fake" dynamically generated timespans.
 * Their set depends on the scheduler visible timespan and changes upon the timespan change.
 *
 * There are few methods allowing to distinguish a recurring event and an occurrence: {@link #property-isRecurring}, {@link #property-isOccurrence}
 * and {@link #property-recurringTimeSpan} (returns the event this record is an occurrence of).
 *
 * The {@link #field-recurrenceRule recurrence rule} defined for the event is parsed and
 * represented with {@link Scheduler.model.RecurrenceModel RecurrenceModel} class (can be changed by setting {@link #property-recurrenceModel} property) instance.
 * See: {@link #property-recurrence} property.
 * @mixin
 * @mixinbase TimeSpan
 */
var RecurringTimeSpan = (Target => class RecurringTimeSpan extends (Target || TimeSpan) {
  static get $name() {
    return 'RecurringTimeSpan';
  }
  /**
   * Returns `true` if this timespan supports recurring.
   * @property {Boolean}
   * @category Recurrence
   */
  get supportsRecurring() {
    return true;
  }
  static get fields() {
    return [
    /**
     * The timespan recurrence rule. A string in [RFC-5545](https://tools.ietf.org/html/rfc5545#section-3.3.10)
     * described format ("RRULE" expression).
     * @field {String} recurrenceRule
     * @category Scheduling
     */
    {
      name: 'recurrenceRule',
      internal: true
    },
    /**
     * A string (either a single date or multiple dates separated by comma) or an array of strings containing
     * the timespan exception dates. The dates that must be skipped when generating occurrences for a repeating
     * timespan. This is used to modify only individual occurrences of the timespan so the further regenerations
     * won't create another copy of this occurrence again.
     *
     * ```javascript
     * {
     *     id: 7,
     *     startDate: '2021-10-12T14:00:00',
     *     endDate: '2021-10-12T15:00:00',
     *     name: 'Lunch',
     *     resourceId: 'hotel',
     *     recurrenceRule: 'FREQ=DAILY;COUNT=5',
     *     exceptionDates: ['2021-10-14']
     * }
     * ```
     *
     * Use {@link #function-addExceptionDate} method to add an individual entry to the dates array:
     *
     * ```javascript
     * // Break the link between the occurrence and its base.
     * // This also adds the occurrence date as an exception date
     * // so that the base timespan knows that this date should be skipped when regenerating its occurrences.
     * occurrence.recurringTimeSpan = null;
     *
     * // now the occurrence is an individual record that can be changed & persisted freely
     * occurrence.setStartEndDate(new Date(2018, 6, 2), new Date(2018, 6, 3));
     * ```
     * **Note:** The dates in this field get automatically removed when the event changes its {@link Scheduler.model.TimeSpan#field-startDate start date}.
     *
     * @field {String|String[]} exceptionDates
     * @category Scheduling
     */
    {
      name: 'exceptionDates',
      convert: convertExceptionDatesValue,
      serialize: serializeExceptionDatesValue,
      internal: true
    }];
  }
  /**
   * Override of {@link Core/data/Model}'s method. If an {@link #property-isOccurrence}
   * is passed, it is detached from its parent recurring event. If it still has a recurrence
   * then the recurring event is changed to stop at the occurrence date. If it has no recurrence
   * an exception is added at the occurrence date.
   * @category Recurrence
   */
  remove() {
    if (this.isOccurrence) {
      const me = this,
        {
          recurringTimeSpan
        } = me;
      me.cancelBatch();
      recurringTimeSpan.beginBatch();
      me.detachFromRecurringEvent();
      recurringTimeSpan.endBatch();
    } else {
      return super.remove(...arguments);
    }
  }
  get eventStore() {
    var _this$firstStore;
    let result = this.isOccurrence ? this.recurringEvent.eventStore : super.eventStore;
    // Recurrence methods are called on `eventStore`, but in case we have a regular
    // store with recurrence mixin, we won't event store and should return own
    // store instead
    if (!result && (_this$firstStore = this.firstStore) !== null && _this$firstStore !== void 0 && _this$firstStore.isRecurringTimeSpansMixin) {
      result = this.firstStore;
    }
    return result;
  }
  /**
   * Name of the class representing the recurrence model, defaults to {@link Scheduler.model.RecurrenceModel}
   * @property {String}
   * @category Recurrence
   */
  get recurrenceModel() {
    return this._recurrenceModel || RecurrenceModel;
  }
  set recurrenceModel(model) {
    this._recurrenceModel = model;
  }
  /**
   * Sets a recurrence for the timespan with a given frequency, interval, and end.
   * @param {RecurrenceModelConfig|String|Scheduler.model.RecurrenceModel} recurrence A data object for an instance of
   * {@link Scheduler.model.RecurrenceModel}. May also be the frequency string: `DAILY`, `WEEKLY`, `MONTHLY`, or
   * `YEARLY`.
   *
   * ```javascript
   * // repeat the event every other week till Jan 2 2039
   * event.setRecurrence({
   *     frequency : "WEEKLY",
   *     interval  : 2,
   *     endDate   : new Date(2039, 0, 2)
   * });
   * ```
   *
   * Also a {@link Scheduler.model.RecurrenceModel recurrence model} can be provided as the only argument for this
   * method:
   *
   * ```javascript
   * const recurrence = new RecurrenceModel({ frequency : 'DAILY', interval : 5 });
   *
   * event.setRecurrence(recurrence);
   * ```
   *
   * or
   *
   * ```javascript
   * event.setRecurrence("WEEKLY", 2, new Date(2039, 0, 2));
   * ```
   * @param {Number} [interval] The interval between occurrences (instances of this recurrence). For example, a daily
   * recurrence with an interval of 2 occurs every other day. Must be greater than 0.
   * @param {Number|Date} [recurrenceEnd] The end of the recurrence. The value can be specified by a date or by a
   * maximum count of occurrences (has to greater than 1, since 1 means the event itself).
   * @category Recurrence
   */
  setRecurrence(recurrence, interval, recurrenceEnd) {
    const me = this;
    let recurrenceRule;
    if (recurrence) {
      if (!recurrence.isRecurrenceModel) {
        if (typeof recurrence === 'string') {
          recurrence = {
            frequency: recurrence
          };
          if (interval) {
            recurrence.interval = interval;
          }
          // If the recurrence is limited
          if (recurrenceEnd) {
            if (recurrenceEnd instanceof Date) {
              recurrence.endDate = recurrenceEnd;
            } else {
              recurrence.count = recurrenceEnd;
            }
          }
        }
        recurrence = new me.recurrenceModel(recurrence);
      }
      // The RecurrenceModel has a reference to its owning recurring event.
      // It uses this to remove the owning event's exceptions after its new endDate
      // whenever its endDate is changed.
      recurrence.timeSpan = me;
      recurrenceRule = recurrence.rule;
    }
    me.recurrence = recurrence;
    me.recurrenceRule = recurrenceRule;
  }
  /**
   * The recurrence model used for the timespan.
   * @property {Scheduler.model.RecurrenceModel}
   * @category Recurrence
   */
  get recurrence() {
    const me = this,
      rule = me.recurrenceRule;
    if (!me._recurrence && rule) {
      me._recurrence = new me.recurrenceModel({
        rule,
        timeSpan: me,
        id: `${me.id}-recurrence`
      });
    }
    return me._recurrence;
  }
  set recurrence(recurrence) {
    const me = this;
    me._recurrence = recurrence;
    if (recurrence) {
      // bind recurrence instance to the model
      recurrence.timeSpan = me;
      me.recurrenceRule = recurrence.rule;
    } else {
      // If this is being done to an occurrence, it's a signal that we are being
      // mutated into an exception. Apply the change immediately, directly to the data.
      if (me.isOccurrence) {
        me.setData('recurrenceRule', null);
      } else {
        me.recurrenceRule = null;
      }
    }
  }
  /**
   * Indicates if the timespan is recurring.
   * @property {Boolean}
   * @readonly
   * @category Recurrence
   */
  get isRecurring() {
    // MUST evaluate in this order so that if it is an occurrence,
    // the recurrence getter does not refresh the rule
    return Boolean(!this.isOccurrence && this.recurrence);
  }
  /**
   * Indicates if the timespan is an occurrence of another recurring timespan.
   * @property {Boolean}
   * @readonly
   * @category Recurrence
   */
  get isOccurrence() {
    return Boolean(this.recurringTimeSpan);
  }
  /**
   * The "main" timespan this model is an occurrence of. For non-occurrences returns `null`.
   * @property {Scheduler.model.TimeSpan}
   * @readonly
   * @internal
   * @category Recurrence
   */
  get recurringTimeSpan() {
    return this._recurringTimeSpan;
  }
  get isPersistable() {
    return super.isPersistable && (!this.supportsRecurring || !this.isOccurrence);
  }
  set recurringTimeSpan(recurringTimeSpan) {
    this._recurringTimeSpan = recurringTimeSpan;
  }
  /**
   * Returns the occurrences of this event over the specified time range. If the first
   * occurrence is in the time range `*this*` record is included in that position.
   * @param {Date} startDate The start date of the range for which to include occurrences.
   * @param {Date} [endDate] The end date of the range for which to include occurrences.
   * Defaults to the startDate.
   * @returns {Scheduler.model.TimeSpan[]} The array of occurrences which occur over the specified range.
   * @category Recurrence
   */
  getOccurrencesForDateRange(startDate, endDate = startDate) {
    return this.eventStore.getOccurrencesForTimeSpan(this, startDate, endDate);
  }
  /**
   * Array of this recurring timespan's cached occurrences. __Not including the owning recurring
   * event__.
   *
   * Empty if the timespan is not recurring.
   *
   * __Note that this is an internal accessor and is cleared whenever changes are made to the
   * owning recurring event__.
   * @property {Scheduler.model.TimeSpan[]}
   * @readonly
   * @internal
   * @category Recurrence
   */
  get occurrences() {
    if (this.isRecurring) {
      const result = [];
      // The occurrencesMap contains entries for each occurrence date.
      this.occurrenceMap.forEach(occurrence => {
        if (occurrence !== this) {
          result.push(occurrence);
        }
      });
      return result;
    }
    return emptyArray;
  }
  /**
   * A Map, keyed by each date an occurrence intersects, of occurrences of this event.
   * @property {Map}
   * @readonly
   * @internal
   * @category Recurrence
   */
  get occurrenceMap() {
    return this._occurrencesMap || (this._occurrencesMap = new Map());
  }
  /**
   * Removes an occurrence from this recurring timespan's cached occurrences.
   * @param dateOrTimeSpan occurrence date or occurrence TimeSpan
   * @internal
   * @category Recurrence
   */
  removeOccurrence(dateOrTimeSpan) {
    var _this$eventStore;
    const date = dateOrTimeSpan.isTimeSpan ? dateOrTimeSpan.occurrenceDate : dateOrTimeSpan;
    // Clear the occurrences *is* we are in an EventStore.
    (_this$eventStore = this.eventStore) === null || _this$eventStore === void 0 ? void 0 : _this$eventStore.globalOccurrences.delete(this.createRecurrenceKey(date));
    // Remove occurrence from its by-startDate cache
    this.occurrenceMap.delete(DateHelper.makeKey(date));
  }
  /**
   * Removes all cached occurrences on or after the passed date from this recurring timespan's cached occurrences.
   * @internal
   * @category Recurrence
   */
  removeOccurrencesFrom(date) {
    this.occurrenceMap.forEach((occurrence, dateKey) => {
      if (DateHelper.parseKey(dateKey) >= date) {
        this.removeOccurrence(occurrence);
      }
    });
  }
  /**
   * Removes this recurring timespan's cached occurrences.
   * @internal
   * @category Recurrence
   */
  removeOccurrences() {
    // This recurring event must also be removed from the occurrenceMap if it's there
    // So insert it as the first element. Can also be found from the store's global occurrence
    // Map using [...this.eventStore.globalOccurrences.keys()].filter(e => e.startsWith(`_generated:${this.id}`))
    [this, ...this.occurrences].forEach(occurrence => this.removeOccurrence(occurrence));
  }
  /**
   * The method is triggered when the timespan recurrence settings get changed.
   * It updates the {@link #field-recurrenceRule} field in this case.
   * @internal
   * @category Recurrence
   */
  onRecurrenceChanged() {
    var _this$recurrence;
    this.recurrenceRule = ((_this$recurrence = this.recurrence) === null || _this$recurrence === void 0 ? void 0 : _this$recurrence.rule) || null;
  }
  sanitizeRecurrenceData(recurrence = this.recurrence) {
    // Remove all exceptionsDates that our owning timeSpan had that are
    // now after our end date and therefore redundant.
    if (recurrence.endDate) {
      const endDate = DateHelper.clearTime(recurrence.endDate),
        {
          exceptionDates
        } = this;
      // Clear any now-invalid cached occurrences
      this.removeOccurrencesFrom(endDate);
      // If we had any exceptions on or after this date, remove them.
      if (exceptionDates) {
        for (const dateKey in exceptionDates) {
          const exceptionDate = DateHelper.parseKey(dateKey);
          if (exceptionDate >= endDate) {
            delete exceptionDates[dateKey];
          }
        }
      }
    }
  }
  /**
   * The original {@lScheduler.model.TimeSpan#field-startDate startDate} of this event before any modifications
   * took place. Used by {@link #function-removeOccurrence} and {@link #function-detachFromRecurringEvent}
   * @internal
   * @readonly
   * @category Recurrence
   */
  get occurrenceDate() {
    var _this$meta$modified;
    return ((_this$meta$modified = this.meta.modified) === null || _this$meta$modified === void 0 ? void 0 : _this$meta$modified.startDate) || this.startDate;
  }
  /**
   * If this event is an {@link #property-isOccurrence occurrence} of a recurring event, then this
   * property yields its zero-based occurrence index in the sequence.
   * @property {Number}
   * @readonly
   * @category Recurrence
   */
  get occurrenceIndex() {
    return AbstractRecurrenceIterator.getOccurrenceIndex(this);
  }
  /**
   * Builds an occurrence of this recurring event by cloning the timespan data.
   * The method is used internally by the __RecurringTimeSpans__ mixin.
   * Override it if you need to customize the generated occurrences.
   *
   * If the date requested is the start date of the event sequence, `this`
   * record is returned. All runs of recurring events begin with the base record.
   * @param  {Date} occurrenceDate The occurrence start date.
   * @param  {Boolean} isFirst `true` if this is the first occurrence.
   * @returns {Scheduler.model.TimeSpan} The occurrence.
   * @internal
   * @category Recurrence
   */
  buildOccurrence(occurrenceDate, isFirst) {
    var _me$eventStore;
    const me = this,
      {
        occurrenceMap,
        recurrence,
        meta
      } = me,
      globalOccurrences = (_me$eventStore = me.eventStore) === null || _me$eventStore === void 0 ? void 0 : _me$eventStore.globalOccurrences,
      occurrenceKey = DateHelper.makeKey(occurrenceDate),
      id = me.createRecurrenceKey(occurrenceDate, occurrenceKey),
      onStartDate = !(occurrenceDate - me.startDate),
      {
        fieldMap
      } = me.constructor;
    // An occurrence has a unique ID which identifies it by its base recurring event and its time.
    let occurrence = globalOccurrences === null || globalOccurrences === void 0 ? void 0 : globalOccurrences.get(id),
      {
        duration
      } = me;
    // If there's no duration, or it's an all day event (which makes the event ceil and floor its
    // start and end dates, but does *NOT* as of 27/5/2020 adjust its duration) then
    // we calculate the duration of the occurrence.
    if (me.endDate && (me.allDay || !duration)) {
      duration = DateHelper.as(me.durationUnit, me.endDate.getTime() - me.startDate.getTime());
    }
    // Don't let NaN in record data
    const occurrenceEndDate = duration !== undefined ? DateHelper.add(occurrenceDate, duration, me.durationUnit) : undefined;
    if (!occurrence) {
      // If this is the first occurrence (start times may not match), or it's right on the start
      // then this recurring event *IS* the occurrence
      if (isFirst || onStartDate) {
        occurrence = me;
        // We are the first occurrence.
        // But if our start time is not as the rule requires, adjust ourself *silently*
        if (!onStartDate) {
          me.setStartEndDate(occurrenceDate, occurrenceEndDate, true);
          // Since we've changed the event start date the recurrence "Days"/"MonthDays"/"Months"
          // might get redundant in case the event start date matches the fields values
          // Calling recurrence sanitize() will clean the fields in this case.
          recurrence.suspendTimeSpanNotifying();
          recurrence.sanitize();
          recurrence.resumeTimeSpanNotifying();
        }
        // Either way, because of adjustment above, or initial correctness, we are in sync
        // with our recurrence rule. A RecurrenceIterator is now able to calculate a correct
        // UNTIL date from a COUNT value. See AbstractRecurrenceIterator#processIterationConfig
        meta.isSyncedWithRule = true;
      }
      // Generate an occurrence which references this as its parent
      else {
        occurrence = me.copy({
          [fieldMap.id.dataSource]: id,
          [fieldMap.startDate.dataSource]: occurrenceDate,
          [fieldMap.endDate.dataSource]: occurrenceEndDate,
          [fieldMap.duration.dataSource]: duration,
          constraintDate: null,
          constraintType: null
        }, {
          creatingOccurrence: true
        });
        occurrence.recurringTimeSpan = me;
      }
      globalOccurrences === null || globalOccurrences === void 0 ? void 0 : globalOccurrences.set(id, occurrence);
      // A recurring timespan keeps a by-startDate index of occurrences.
      // And itself will be among those.
      occurrenceMap.set(occurrenceKey, occurrence);
    }
    return occurrence;
  }
  createRecurrenceKey(date = this.startDate, dateKey = null) {
    return `_generated:${this.id}:${dateKey || DateHelper.makeKey(date)}`;
  }
  // Converts this occurrence to a new "master" event
  convertToRealEvent(wasSet, silent) {
    var _wasSet$resourceRecor;
    if (!this.isOccurrence) {
      return;
    }
    const me = this,
      {
        recurringTimeSpan,
        resource,
        occurrenceIndex,
        recurrence
      } = me,
      count = recurrence && recurringTimeSpan.recurrence.count,
      // resourceRecords is a temporary property of occurrence events to handle cases
      // if only resources has been updated. (change only resources won't mark record as dirty)
      newResource = (wasSet === null || wasSet === void 0 ? void 0 : (_wasSet$resourceRecor = wasSet.resourceRecords) === null || _wasSet$resourceRecor === void 0 ? void 0 : _wasSet$resourceRecor.value) || me.data.newResource;
    recurringTimeSpan.beginBatch();
    me.detachFromRecurringEvent();
    me.clearChanges();
    // Must silently set our own ID, not be the key generated from our parent id and occurrence date.
    // Must not result in the id field being in the modified state.
    me.setData('id', me.generateId(recurringTimeSpan.eventStore));
    if (newResource) {
      // clear resourceId to avoid auto-adding to assignmentStore, it is handled manually bellow
      delete me.data.resourceId;
    }
    // The impending changes to the former parent recurring event trigger a full refresh.
    recurringTimeSpan.eventStore.add(me, silent);
    // Ensure that the original count is honoured.
    // If we are the 8th occurrence of 10, OUR repeat count must be 3.
    if (count) {
      me.recurrence.count = count - occurrenceIndex;
    }
    if (newResource || resource) {
      me.assign(newResource || resource);
    }
    // remove data after apply
    if (newResource) {
      delete me.data.resourceRecords;
    }
    if (wasSet) {
      delete wasSet.resourceRecords;
    }
    // Any change to a recurring events triggers a store refresh event.
    recurringTimeSpan.endBatch();
  }
  afterChange(toSet, wasSet, silent, ...args) {
    const me = this,
      {
        eventStore
      } = me;
    // reset cached recurrence instance in case "recurrenceRule" is changed
    if ('recurrenceRule' in wasSet) {
      me._recurrence = null;
      // If we are a recurring event, we must be in the recurringEvents cache.
      // If we are *not* a recurring event, we must *not* be in there.
      // An event not yet in a store (eg dragging to create) won't have an eventStore.
      eventStore === null || eventStore === void 0 ? void 0 : eventStore.recurringEvents[wasSet.recurrenceRule.value ? 'add' : 'delete'](me);
    }
    // Any change to an occurrence adds it to an event store, at which point
    // it ceases to be an occurrence.
    //
    // If it has a recurrenceRule it becomes the start of a new recurring event series,
    // and the old owning recurring event stops on the day before.
    //
    // If it has no recurrenceRule, it becomes an exception to its owning recurring event.
    if (me.isOccurrence) {
      me.convertToRealEvent(wasSet, silent);
    }
    // Setting a newException date must mark the exceptionDates as modified
    else if ('newExceptionDate' in wasSet) {
      me.meta.modified.exceptionDates = true;
      delete me.meta.modified.newExceptionDate;
      // Remove any occurrence on that date from our by-startDate cache
      // and from the global occurrences cache
      me.removeOccurrence(wasSet.newExceptionDate.value);
    }
    return super.afterChange(toSet, wasSet, silent, ...args);
  }
  /**
   * Detaches an occurrence from its owning recurring event so that it can be added to the eventStore
   * either as an exception, or as the start of a new recurring sequence.
   * @internal
   * @category Recurrence
   */
  detachFromRecurringEvent() {
    const me = this,
      // For access further down, breaking the link involves engine if trying to get the occurrenceDate later,
      // resulting in the wrong date
      {
        recurringTimeSpan,
        occurrenceDate,
        startDate
      } = me;
    // Break the link
    me.recurringTimeSpan = null;
    // The occurrenceDate is injected into the data when an occurrence is created.
    // the recurringTimeSpan's afterChange will remove any cache occurrence
    // for this date; see above
    recurringTimeSpan.addExceptionDate(occurrenceDate);
    // If we still have a recurrenceRule, we're being promoted to be a new recurring event.
    // The recurrence setter applies the rule immediately to occurrences, so this will
    // always be correct.
    if (me.recurrenceRule) {
      // The RecurrenceModel removes occurrences and exceptions after this date
      recurringTimeSpan.recurrence.endDate = DateHelper.add(startDate, -1, 'minute');
    }
  }
  /**
   * The setter used by Model#inSet when {@link #function-addExceptionDate} is called.
   * Adding an exception must trigger change processing in a recurring event, so it must
   * be changed through a {@link Core.data.Model#function-set} call. Also, the change must be batchable
   * with other changes.
   * @private
   * @readonly
   * @category Recurrence
   */
  set newExceptionDate(date) {
    if (date) {
      const exceptionDates = this.exceptionDates || (this.exceptionDates = {});
      exceptionDates[DateHelper.makeKey(date)] = 1;
    }
  }
  /**
   * Adds an exception date that should be skipped when generating occurrences for the timespan.
   * The methods adds an entry to the array kept in {@link #field-exceptionDates} field.
   * @param {Date} date Exception date.
   * @internal
   * @category Recurrence
   */
  addExceptionDate(newExceptionDate) {
    return this.set({
      newExceptionDate
    });
  }
  /**
   * Does this recurring event have an exception on the passed date.
   * @param {Date} date The date to find an exception for.
   * @returns {Boolean} `true` if the event has an exception starting on the passed date.
   * @category Recurrence
   */
  hasException(date) {
    var _this$exceptionDates;
    return (_this$exceptionDates = this.exceptionDates) === null || _this$exceptionDates === void 0 ? void 0 : _this$exceptionDates[DateHelper.makeKey(date)];
  }
});

const oneDayMS = 1000 * 60 * 60 * 24;
/**
 * @module Scheduler/model/mixin/EventModelMixin
 */
/**
 * Mixin that holds configuration shared between events in Scheduler and Scheduler Pro.
 * @mixin
 */
var EventModelMixin = (Target => class EventModelMixin extends Target {
  static get $name() {
    return 'EventModelMixin';
  }
  // Flag checked by EventStore to make sure it uses a valid subclass
  static get isEventModel() {
    return true;
  }
  /**
   * Set value for the specified field(s), triggering engine calculations immediately. See
   * {@link Core.data.Model#function-set Model#set()} for arguments.
   *
   * ```javascript
   * eventRecord.set('duration', 4);
   * // eventRecord.endDate is not yet calculated
   *
   * await eventRecord.setAsync('duration', 4);
   * // eventRecord.endDate is calculated
   * ```
   *
   * @param {String|Object} field The field to set value for, or an object with multiple values to set in one call
   * @param {*} value Value to set
   * @param {Boolean} [silent=false] Set to true to not trigger events. If event is recurring, occurrences won't be updated
   * automatically.
   * @function setAsync
   * @category Editing
   * @async
   */
  //region Fields
  static get fields() {
    return [
    /**
     * The start date of a time span (or Event / Task).
     *
     * Uses {@link Core/helper/DateHelper#property-defaultFormat-static DateHelper.defaultFormat} to convert a
     * supplied string to a Date. To specify another format, either change that setting or subclass TimeSpan and
     * change the dateFormat for this field.
     *
     * UI fields representing this data field are disabled for summary tasks. See {@link #function-isEditable}
     * for details.
     *
     * Note that the field always returns a `Date`.
     *
     * Also note that modifying the `startDate` at runtime will move the event in time, without affecting its
     * duration (with reservation for other scheduling logic affecting the duration). If you want to change the
     * `startDate` and `duration`, use {@link Scheduler/model/TimeSpan#function-setStartDate} instead (passing
     * `false` as the second argument).
     *
     * @field {Date} startDate
     * @accepts {String|Date}
     * @category Scheduling
     */
    /**
     * The end date of a time span (or Event / Task).
     *
     * Uses {@link Core/helper/DateHelper#property-defaultFormat-static DateHelper.defaultFormat} to convert a
     * supplied string to a Date. To specify another format, either change that setting or subclass TimeSpan and
     * change the dateFormat for this field.
     *
     * UI fields representing this data field are disabled for summary tasks. See {@link #function-isEditable} for details.
     *
     * Note that the field always returns a `Date`.
     *
     * @field {Date} endDate
     * @accepts {String|Date}
     * @category Scheduling
     */
    /**
     * The numeric part of the timespan's duration (the number of units).
     *
     * UI fields representing this data field are disabled for summary tasks. See {@link #function-isEditable}
     * for details.
     *
     * @field {Number} duration
     * @category Scheduling
     */
    /**
     * Calculated field which encapsulates the duration's magnitude and unit. This field will not be persisted,
     * setting it will update the {@link #field-duration} and
     * {@link Scheduler.model.TimeSpan#field-durationUnit} fields.
     *
     * UI fields representing this data field are disabled for summary tasks. See {@link #function-isEditable}
     * for details.
     *
     * @field {DurationConfig|Core.data.Duration} fullDuration
     * @category Scheduling
     */
    /**
     * The unique identifier of a task (mandatory)
     * @field {String|Number} id
     * @category Common
     */
    /**
     * Id of the resource this event is associated with (only usable for single assignments). We recommend
     * using assignments in an AssignmentStore over this approach. Internally any Event using `resourceId`
     * will have an assignment in AssignmentStore generated.
     * @field {String|Number} resourceId
     * @category Common
     */
    {
      name: 'resourceId',
      internal: true
    },
    /**
     * The array of {@link Scheduler.model.ResourceModel resources} which are assigned to this event.
     * @field {String|Number} resources
     * @category Common
     */
    {
      name: 'resources',
      column: {
        type: 'resourceassignment'
      },
      persist: false,
      internal: true
    },
    /**
     * Specify false to prevent the event from being dragged (if EventDrag feature is used)
     * @field {Boolean} draggable
     * @default true
     * @category Interaction
     */
    {
      name: 'draggable',
      type: 'boolean',
      persist: false,
      defaultValue: true,
      internal: true
    },
    /**
     * Specify `false` to prevent the event from being resized (if EventResize feature is used). You can also
     * specify `'start'` or `'end'` to only allow resizing in one direction
     * @field {Boolean|String} resizable
     * @default true
     * @category Interaction
     */
    {
      name: 'resizable',
      persist: false,
      defaultValue: true,
      internal: true
    },
    // true, false, 'start' or 'end'
    /**
     * A field marking event as all day(s) spanning event.
     * For example, a holiday day may be represented by a `startDate`, and the `allDay` flag.
     * @field {Boolean} allDay
     * @category Scheduling
     */
    {
      name: 'allDay',
      type: 'boolean',
      defaultValue: false
    },
    /**
     * Controls this events appearance, see Schedulers
     * {@link Scheduler.view.mixin.TimelineEventRendering#config-eventStyle eventStyle config} for
     * available options.
     * @field {'plain'|'border'|'colored'|'hollow'|'line'|'dashed'|'minimal'|'rounded'|'calendar'|'interday'|null} eventStyle
     * @category Styling
     */
    {
      name: 'eventStyle',
      internal: true
    },
    /**
     * Controls the primary color of the event, see Schedulers
     * {@link Scheduler.view.mixin.TimelineEventRendering#config-eventColor eventColor config} for
     * available colors.
     * @field {'red'|'pink'|'purple'|'violet'|'indigo'|'blue'|'cyan'|'teal'|'green'|'lime'|'yellow'|'orange'|'deep-orange'|'gray'|'gantt-green'|String|null} eventColor
     * @category Styling
     */
    {
      name: 'eventColor',
      internal: true
    },
    /**
     * Width (in px) to use for this milestone when using Scheduler#milestoneLayoutMode 'data'.
     * @field {Number} milestoneWidth
     * @category Styling
     */
    {
      name: 'milestoneWidth',
      internal: true
    },
    /**
     * Set this field to `false` to opt out of {@link Scheduler.feature.StickyEvents sticky event content}
     * (keeping event text in view while scrolling).
     * @field {Boolean} stickyContents
     * @category Styling
     */
    {
      name: 'stickyContents',
      internal: true
    }];
  }
  //endregion
  //region Id change
  updateAssignmentEventIds() {
    this.assigned.forEach(assignment => {
      assignment.eventId = this.id;
    });
  }
  syncId(value) {
    super.syncId(value);
    this.updateAssignmentEventIds();
  }
  //endregion
  //region Resources
  /**
   * Returns all resources assigned to an event.
   *
   * @property {Scheduler.model.ResourceModel[]}
   * @category Assignments & Resources
   * @readonly
   */
  get resources() {
    // Only include valid resources, to not have nulls in the result
    return this.assignments.reduce((resources, {
      resource
    }) => {
      resource && resources.push(resource.$original);
      return resources;
    }, []);
  }
  set resources(resources) {
    resources = ArrayHelper.asArray(resources);
    const me = this,
      newResourceIds = resources.map(me.constructor.asId);
    if (me.usesSingleAssignment) {
      me.set('resourceId', newResourceIds[0]);
    } else {
      const existingResourceIds = me.assignments.map(a => a.resource.id),
        {
          onlyInA: toAdd,
          onlyInB: toRemove
        } = ArrayHelper.delta(newResourceIds, existingResourceIds);
      // Add first, remove after. Otherwise event might get removed with its last assignment
      me.assignmentStore.add(toAdd.map(resourceId => ({
        resource: resourceId,
        event: me
      })));
      me.assignmentStore.remove(toRemove.map(resourceId => me.assignments.find(a => a.resource.id === resourceId)));
    }
  }
  /**
   * Iterate over all associated resources
   * @private
   * @category Assignments & Resources
   */
  forEachResource(fn, thisObj = this) {
    for (const resource of this.resources) {
      if (fn.call(thisObj, resource) === false) {
        return;
      }
    }
  }
  /**
   * Returns either the resource associated with this event (when called w/o `resourceId`) or resource
   * with specified id.
   *
   * @param {String} [resourceId] To retrieve a specific resource
   * @returns {Scheduler.model.ResourceModel}
   * @category Assignments & Resources
   */
  getResource(resourceId) {
    if (resourceId == null) {
      return this.resource;
    }
    return this.resourceStore ? this.resourceStore.getById(resourceId) : null;
  }
  //endregion
  //region Dates
  get startDate() {
    let dt;
    if (this.isOccurrence) {
      dt = this.get('startDate');
    } else {
      // Micro optimization to avoid expensive super call. super will be hit in Scheduler Pro
      dt = this._startDate ?? super.startDate;
    }
    if (this.allDay) {
      dt = this.constructor.getAllDayStartDate(dt);
    }
    return dt;
  }
  set startDate(startDate) {
    if (this.batching) {
      this._startDate = startDate;
      this.set({
        startDate
      });
    } else {
      super.startDate = startDate;
    }
  }
  get endDate() {
    let dt;
    if (this.isOccurrence) {
      dt = this.get('endDate');
    } else {
      // Micro optimization to avoid expensive super call. super will be hit in Scheduler Pro
      dt = this._endDate ?? super.endDate;
    }
    if (this.allDay) {
      dt = this.constructor.getAllDayEndDate(dt);
    }
    return dt;
  }
  set endDate(endDate) {
    if (this.batching) {
      this._endDate = endDate;
      this.set({
        endDate
      });
    } else {
      super.endDate = endDate;
    }
  }
  // Cannot use `convert` method because it might be disabled by `useRawData : true` and we always need to calculate
  // that value
  get wrapStartDate() {
    return this.startDate;
  }
  set wrapStartDate(value) {}
  get wrapEndDate() {
    return this.endDate;
  }
  set wrapEndDate(value) {}
  /**
   * Shift the dates for the date range by the passed amount and unit
   * @param {'ms'|'s'|'m'|'h'|'d'|'w'|'M'|'y'} unit The unit to shift by, see {@link Core.helper.DateHelper}
   * for more information on valid formats.
   * @param {Number} amount The amount to shift
   * @returns {Promise} A promise which is resolved when shift calculations are done
   * @async
   * @method shift
   * @category Scheduling
   */
  //endregion
  //region Is
  // Used internally to differentiate between Event and ResourceTimeRange
  get isEvent() {
    return true;
  }
  /**
   * Returns true if event can be drag and dropped
   * @property {Boolean}
   * @category Editing
   */
  get isDraggable() {
    return !this.readOnly && this.draggable;
  }
  /**
   * Returns true if event can be resized, but can additionally return 'start' or 'end' indicating how this event can
   * be resized.
   *
   * Milestones and parent events (that are not manuallyScheduled) cannot be resized.
   *
   * @property {Boolean|String}
   * @readonly
   * @category Editing
   */
  get isResizable() {
    return !this.isMilestone && (!this.isParent || this.manuallyScheduled) && this.resizable;
  }
  /**
   * Returns false if the event is not persistable. By default it always is, override this getter if you need
   * custom logic.
   *
   * @property {Boolean}
   * @readonly
   * @category Editing
   */
  get isPersistable() {
    // Records not yet fully created cannot be persisted
    return super.isPersistable && !this.isCreating;
  }
  endBatch() {
    const me = this,
      {
        isPersistable: wasPersistable,
        meta: {
          batchChanges
        }
      } = me;
    // Remove cached values
    // https://github.com/bryntum/support/issues/3358
    if (batchChanges) {
      if ('endDate' in batchChanges) {
        delete me._endDate;
        // When project recalculates start/end date and committing changes to record it calls endBatch. In this
        // case wrap dates become invalid and should be reset on the record. We do it by forcing `null` value.
        // Possible scenarios include adding dependency or moving first event in a dependency chain.
        // Covered by pro/features/BufferTimeDependencies.t
        // wrap date is an internal field we should be fine without mapping
        if (me.postamble) {
          me.wrapEndDate = null;
        }
      }
      if ('startDate' in batchChanges) {
        delete me._startDate;
        if (me.preamble) {
          me.wrapStartDate = null;
        }
      }
    }
    super.endBatch(...arguments);
    // If this event newly persistable, its assignments are eligible for syncing.
    if (me.isPersistable && !wasPersistable && !me.ignoreBag && me.assigned) {
      for (const assignment of me.assigned) {
        assignment.stores.forEach(s => s.updateModifiedBagForRecord(assignment));
      }
    }
  }
  get isCreating() {
    return super.isCreating;
  }
  set isCreating(value) {
    super.isCreating = value;
    this.assignments.forEach(record => record.isCreating = value);
  }
  //endregion
  //region Single assignment compatibility
  get usesSingleAssignment() {
    return !this.eventStore || this.eventStore.usesSingleAssignment;
  }
  copy(...args) {
    const copy = super.copy(...args);
    // Don't copy resourceId when using multi assignment
    if (!this.usesSingleAssignment) {
      copy.resourceId = null;
    }
    return copy;
  }
  /**
   * Override persistable getter to prevent sending resourceId when using multiple resource assignment mode
   * https://github.com/bryntum/support/issues/1345
   * @private
   */
  get persistableData() {
    const data = super.persistableData;
    if (!this.usesSingleAssignment) {
      delete data.resourceId;
    }
    return data;
  }
  /**
   * Returns the first assigned resource, or assigns a resource
   * @member {Scheduler.model.ResourceModel} resource
   * @category Assignments & Resources
   */
  get resource() {
    const {
      resources
    } = this;
    return resources.length ? resources[0] : null;
  }
  set resource(resourceRecord) {
    // Use the resourceId setter for single assignment
    this.resourceId = this.constructor.asId(resourceRecord);
  }
  get resourceId() {
    var _this$resource;
    return this.usesSingleAssignment ? this.get('resourceId') : (_this$resource = this.resource) === null || _this$resource === void 0 ? void 0 : _this$resource.id;
  }
  set resourceId(resourceId) {
    this.applyResourceId(resourceId);
  }
  // Resources + any links to any of them
  get $linkedResources() {
    var _this$resources;
    return ((_this$resources = this.resources) === null || _this$resources === void 0 ? void 0 : _this$resources.flatMap(resourceRecord => [resourceRecord, ...resourceRecord.$links])) ?? [];
  }
  applyResourceId(resourceId, fromApplyValue = false) {
    const me = this,
      {
        eventStore,
        assignments
      } = me;
    // When part of an EventStore, resourceIds are changed to be AssignmentModels
    if (eventStore) {
      var _eventStore$crudManag;
      // If not yet part of project, we cant resolve assignmentStore from ourselves
      const assignmentStore = eventStore.assignmentStore || ((_eventStore$crudManag = eventStore.crudManager) === null || _eventStore$crudManag === void 0 ? void 0 : _eventStore$crudManag.assignmentStore);
      if (resourceId != null) {
        if (!me.meta.skipEnforcingSingleAssignment) {
          eventStore.usesSingleAssignment = true;
        }
        // Reassign if already assigned, only single assignment allowed
        if (assignments !== null && assignments !== void 0 && assignments.length && resourceId !== assignments[0].resourceId) {
          //assignments[0].set('resourceId', resourceId, Boolean(me.eventStore.eventsSuspended));
          // Silent reassign if events are suspended on event store, won't be expecting UI update then
          const eventsSuspended = Boolean(eventStore.eventsSuspended);
          eventsSuspended && assignmentStore.suspendEvents();
          assignments[0].resource = resourceId;
          eventsSuspended && assignmentStore.resumeEvents();
        }
        // Otherwise assign
        else {
          assignmentStore.assignEventToResource(me, resourceId);
        }
      } else {
        // Setting resourceId to null removes all assignments
        assignmentStore.remove(assignments);
      }
    }
    // Not part of an EventStore, edge case. Set to data unless we are in such operation already
    else if (!fromApplyValue) {
      me.set({
        resourceId
      });
    }
  }
  // Special handling of setting resourceId, creates assignment
  applyValue(useProp, mapping, value, skipAccessors, field) {
    if (field && field.name === 'resourceId' && !this.meta.isAssigning) {
      const {
        eventStore
      } = this;
      eventStore && (eventStore.isAssigning = true);
      this.applyResourceId(value, true);
      eventStore && (eventStore.isAssigning = false);
    }
    super.applyValue(useProp, mapping, value, skipAccessors, field);
  }
  //endregion
  //region Assignment
  /**
   * Returns all assignments for the event. Event must be part of the store for this method to work.
   * @property {Scheduler.model.AssignmentModel[]}
   * @readonly
   * @category Assignments & Resources
   */
  get assignments() {
    return [...(this.assigned || [])];
  }
  /**
   * Assigns this event to the specified resource.
   *
   * *Note:* The event must be part of an EventStore for this to work. If the EventStore uses single assignment
   * (loaded using resourceId) existing assignments will always be removed.
   *
   * @param {Scheduler.model.ResourceModel|String|Number|Scheduler.model.ResourceModel[]|String[]|Number[]} resource A new resource for this event, either as a full
   *        Resource record or an id (or an array of such).
   * @param {Boolean} [removeExistingAssignments] `true` to first remove existing assignments
   * @category Assignments & Resources
   */
  assign(resource, removeExistingAssignments = false) {
    const {
      eventStore
    } = this;
    if (eventStore && !eventStore.usesSingleAssignment) {
      eventStore.assignEventToResource(this, resource, removeExistingAssignments);
    } else {
      // Remember what resource to assign,  directly in single assignment mode or for later when we are joined to
      // an EventStore
      this.resourceId = this.constructor.asId(resource);
      if (!eventStore) {
        // Prevent flagging EventStore as using single assignment when that happens, we cannot know that here
        this.meta.skipEnforcingSingleAssignment = true;
      }
    }
  }
  /**
   * Unassigns this event from the specified resource
   *
   * @param {Scheduler.model.ResourceModel|String|Number} [resource] The resource to unassign from.
   * @category Assignments & Resources
   */
  unassign(resource, removingResource = false) {
    var _me$eventStore;
    const me = this;
    resource = me.constructor.asId(resource);
    // If unassigned is caused by removing the resource the UI should be able to find out to not do extra redraws etc.
    me.meta.removingResource = removingResource;
    (_me$eventStore = me.eventStore) === null || _me$eventStore === void 0 ? void 0 : _me$eventStore.unassignEventFromResource(me, resource);
    me.meta.removingResource = null;
  }
  /**
   * Reassigns an event from an old resource to a new resource
   *
   * @param {Scheduler.model.ResourceModel|String|Number} oldResourceId A resource to unassign from or its id
   * @param {Scheduler.model.ResourceModel|String|Number} newResourceId A resource to assign to or its id
   * @category Assignments & Resources
   */
  reassign(oldResourceId, newResourceId) {
    this.eventStore && this.eventStore.reassignEventFromResourceToResource(this, oldResourceId, newResourceId);
  }
  /**
   * Returns true if this event is assigned to a certain resource.
   *
   * @param {Scheduler.model.ResourceModel|String|Number} resource The resource to query for
   * @returns {Boolean}
   * @category Assignments & Resources
   */
  isAssignedTo(resource) {
    const resourceId = this.constructor.asId(resource);
    return this.assignments.some(assignment => assignment.resourceId === resourceId);
  }
  //endregion
  //region Dependencies
  /**
   * Returns all predecessor dependencies of this event
   *
   * @readonly
   * @property {Scheduler.model.DependencyBaseModel[]}
   * @category Dependencies
   */
  get predecessors() {
    return [...(this.incomingDeps ?? [])];
  }
  /**
   * Returns all successor dependencies of this event
   *
   * @readonly
   * @property {Scheduler.model.DependencyBaseModel[]}
   * @category Dependencies
   *
   */
  get successors() {
    return [...(this.outgoingDeps ?? [])];
  }
  get dependencies() {
    // Dependency feature listens for assignment changes pre commit, thus we risk deps not being set up yet
    return [...(this.incomingDeps ?? []), ...(this.outgoingDeps ?? [])];
  }
  //endregion
  normalize() {
    // Normalization handled by Engine
  }
  inSetNormalize() {
    // Normalization handled by Engine
  }
  /**
   * The "main" event this model is an occurrence of.
   * Returns `null` for non-occurrences.
   * @property {Scheduler.model.EventModel}
   * @alias #Scheduler.model.mixin.RecurringTimeSpan#property-recurringTimeSpan
   * @readonly
   * @category Scheduling
   */
  get recurringEvent() {
    return this.recurringTimeSpan;
  }
  /**
   * Flag which indicates that this event is an interday event. This means that it spans
   * an entire day or multiple days.
   *
   * This is essentially used by the Calendar package to determine if an event should
   * go into the all day zone of a DayView.
   *
   * @property {Boolean}
   * @readonly
   * @category Scheduling
   */
  get isInterDay() {
    const {
      durationMS
    } = this;
    // A full day (86400000 or more) marks as it as interDay,
    // which means it belongs in the all day row of a Calendar DayView
    if (durationMS >= oneDayMS || !durationMS && this.allDay) {
      return true;
    }
    // Working out whether it crosses midnight is a little more difficult
    const {
        endDate,
        startDate
      } = this,
      eventStartMidnight = DateHelper.clearTime(startDate);
    // If either is null or NaN, we have to answer falsy
    if (startDate && endDate) {
      eventStartMidnight.setDate(eventStartMidnight.getDate() + 1);
      // If the endDate is past midnight, it's interDay and goes in the all day row of a Calendar DayView
      return (endDate || DateHelper.add(startDate, durationMS)) > eventStartMidnight;
    }
  }
  //region All day statics
  static getAllDayStartDate(dt) {
    if (dt && dt.isEvent) {
      dt = dt.get('startDate');
    }
    if (dt) {
      dt = DateHelper.clearTime(dt, true);
    }
    return dt;
  }
  static getAllDayEndDate(dt) {
    if (dt && dt.isEvent) {
      dt = dt.get('endDate');
    }
    if (dt && (dt.getHours() > 0 || dt.getMinutes() > 0 || dt.getSeconds() > 0 || dt.getMilliseconds() > 0)) {
      dt = DateHelper.getNext(dt, 'd', 1);
    }
    return dt;
  }
  static getAllDayDisplayStartDate(dt) {
    if (dt && dt.isEvent) {
      dt = dt.get('startDate');
    }
    return DateHelper.clearTime(dt, true);
  }
  static getAllDayDisplayEndDate(startDate, endDate) {
    if (startDate && startDate.isEvent) {
      endDate = startDate.get('endDate');
      startDate = startDate.get('startDate');
    }
    if (endDate) {
      startDate = this.constructor.getAllDayDisplayStartDate(startDate);
      // If date falls on start of the day - subtract one day to show end date correctly
      // e.g. event starts on 2017-01-01 00:00 and ends on 2017-01-02 00:00, editor should show
      // 2017-01-01 for both start and end
      if (DateHelper.clearTime(endDate, true).valueOf() === endDate.valueOf()) {
        endDate = DateHelper.add(endDate, DateHelper.DAY, -1);
      } else if (startDate.valueOf() !== endDate.valueOf()) {
        endDate = DateHelper.clearTime(endDate, true);
      }
    }
    return endDate;
  }
  /**
   * Defines if the given event field should be manually editable in UI.
   * You can override this method to provide your own logic.
   *
   * By default, the method defines {@link #field-endDate}, {@link #field-duration} and {@link #field-fullDuration}
   * fields editable for leaf events only (in case the event is part of a tree store) and all other fields as
   * editable.
   *
   * @param {String} fieldName Name of the field
   * @returns {Boolean} Returns `true` if the field is editable, `false` if it is not and `undefined` if the event has
   * no such field.
   * @category Editing
   */
  isEditable(fieldName) {
    switch (fieldName) {
      // end/duration is allowed to edit for leafs
      case 'endDate':
      case 'duration':
      case 'fullDuration':
        return this.isLeaf;
    }
    return super.isEditable(fieldName);
  }
  //endregion
});

const EngineMixin$4 = SchedulerCoreEvent;
/**
 * @module Scheduler/model/EventModel
 */
/**
 * This class represent a single event in your schedule, usually added to a {@link Scheduler.data.EventStore}.
 *
 * It is a subclass of the {@link Scheduler.model.TimeSpan}, which is in turn subclass of {@link Core.data.Model}.
 * Please refer to documentation of that class to become familiar with the base interface of the event.
 *
 * ## Async date calculations
 *
 * A record created from an {@link Scheduler/model/EventModel} is normally part of an {@link Scheduler.data.EventStore},
 * which in turn is part of a project. When dates or the duration of an event is changed, the project performs async calculations
 * to normalize the other fields.
 * For example if {@link #field-duration} is changed, it will calculate {@link #field-endDate}.
 *
 * As a result of this being an async operation, the values of other fields are not guaranteed to be up to date
 * immediately after a change. To ensure data is up to date, await the calculations to finish.
 *
 * For example, {@link #field-endDate} is not up to date after this operation:
 *
 * ```javascript
 * eventRecord.duration = 5;
 * // endDate not yet calculated
 * ```
 *
 * But if calculations are awaited it is up to date:
 *
 * ```javascript
 * eventRecord.duration = 5;
 * await eventRecord.project.commitAsync();
 * // endDate is calculated
 * ```
 *
 * As an alternative, you can also use `setAsync()` to trigger calculations directly after the change:
 *
 * ```javascript
 * await eventRecord.setAsync({ duration : 5});
 * // endDate is calculated
 * ```
 *
 * ## Subclassing the Event model class
 * The Event model has a few predefined fields as seen below. If you want to add new fields or change the options for the existing fields,
 * you can do that by subclassing this class (see example below).
 *
 * ```javascript
 * class MyEvent extends EventModel {
 *
 *     static get fields() {
 *         return [
 *            // Add new field
 *            { name: 'myField', type : 'number', defaultValue : 0 }
 *         ];
 *     },
 *
 *     myCheckMethod() {
 *         return this.myField > 0
 *     },
 *
 *     ...
 * });
 * ```
 *
 * If you in your data want to use other names for the {@link #field-startDate}, {@link #field-endDate}, {@link #field-resourceId} and name fields you can configure
 * them as seen below:
 *
 * ```javascript
 * class MyEvent extends EventModel {
 *
 *     static get fields() {
 *         return [
 *            { name: 'startDate', dataSource : 'taskStart' },
 *            { name: 'endDate', dataSource : 'taskEnd', format: 'YYYY-MM-DD' },
 *            { name: 'resourceId', dataSource : 'userId' },
 *            { name: 'name', dataSource : 'taskTitle' },
 *         ];
 *     },
 *     ...
 * });
 * ```
 *
 * Please refer to {@link Core.data.Model} for additional details.
 *
 * @extends Scheduler/model/TimeSpan
 * @mixes Scheduler/model/mixin/RecurringTimeSpan
 * @mixes Scheduler/model/mixin/EventModelMixin
 */
class EventModel extends EngineMixin$4.derive(TimeSpan).mixin(RecurringTimeSpan, PartOfProject, EventModelMixin) {
  static get $name() {
    return 'EventModel';
  }
}
EventModel.exposeProperties();
EventModel._$name = 'EventModel';

const EngineMixin$3 = PartOfProject(CoreEventStoreMixin.derive(AjaxStore));
/**
 * @module Scheduler/data/EventStore
 */
/**
 * A store holding all the {@link Scheduler.model.EventModel events} to be rendered into a {@link Scheduler.view.Scheduler Scheduler}.
 *
 * This store only accepts a model class inheriting from {@link Scheduler.model.EventModel}.
 *
 * An EventStore is usually connected to a project, which binds it to other related stores (AssignmentStore,
 * ResourceStore and DependencyStore). The project also handles normalization/calculation of the data on the records in
 * the store. For example if a record is added with a `startDate` and an `endDate`, it will calculate the `duration`.
 *
 * The calculations happens async, records are not guaranteed to have up to date data until they are finished. To be
 * certain that calculations have finished, call `await project.commitAsync()` after store actions. Or use one of the
 * `xxAsync` functions, such as `loadDataAsync()`.
 *
 * Using `commitAsync()`:
 *
 * ```javascript
 * eventStore.data = [{ startDate, endDate }, ...];
 *
 * // duration of the record is not yet calculated
 *
 * await eventStore.project.commitAsync();
 *
 * // now it is
 * ```
 *
 * Using `loadDataAsync()`:
 *
 * ```javascript
 * await eventStore.loadDataAsync([{ startDate, endDate }, ...]);
 *
 * // duration is calculated
 * ```
 *
 * ## Using recurring events
 * When recurring events are in the database, **all recurring event definitions** which started before
 * the requested start date, and have not yet finished recurring MUST be loaded into the EventStore.
 *
 * Only the **base** recurring event **definitions** are stored in the EventStore. You do not
 * need to calculate the future occurrence dates of these events. This is all handled by the EventStore.
 *
 * When asked to yield a set of events for a certain date range for creating a UI through
 * {@link #function-getEvents}, the EventStore *automatically* interpolates any occurrences of
 * recurring events into the results. They do not occupy slots in the EventStore for every date
 * in their repetition range (that would be very inefficient, and *might* be infinite).
 *
 * @mixes Scheduler/data/mixin/PartOfProject
 * @mixes Scheduler/data/mixin/SharedEventStoreMixin
 * @mixes Scheduler/data/mixin/EventStoreMixin
 * @mixes Scheduler/data/mixin/RecurringEventsMixin
 * @mixes Scheduler/data/mixin/GetEventsMixin
 * @extends Core/data/AjaxStore
 */
class EventStore extends EngineMixin$3.mixin(SharedEventStoreMixin, RecurringEventsMixin, EventStoreMixin, DayIndexMixin, GetEventsMixin) {
  static $name = 'EventStore';
  static get defaultConfig() {
    return {
      /**
       * Class used to represent records
       * @config {Scheduler.model.EventModel}
       * @typings {typeof EventModel}
       * @default
       * @category Common
       */
      modelClass: EventModel
    };
  }
}
EventStore._$name = 'EventStore';

/**
 * @module Scheduler/model/DependencyBaseModel
 */
const canonicalDependencyTypes = ['SS', 'SF', 'FS', 'FF'];
/**
 * Base class used for both Scheduler and Gantt. Not intended to be used directly
 *
 * @extends Core/data/Model
 */
class DependencyBaseModel extends Model {
  static get $name() {
    return 'DependencyBaseModel';
  }
  /**
   * Set value for the specified field(s), triggering engine calculations immediately. See
   * {@link Core.data.Model#function-set Model#set()} for arguments.
   **
   * ```javascript
   * dependency.set('from', 2);
   * // dependency.fromEvent is not yet up to date
   *
   * await dependency.setAsync('from', 2);
   * // dependency.fromEvent is up to date
   * ```
   *
   * @param {String|Object} field The field to set value for, or an object with multiple values to set in one call
   * @param {*} value Value to set
   * @param {Boolean} [silent=false] Set to true to not trigger events
   * automatically.
   * @function setAsync
   * @category Editing
   * @async
   */
  //region Fields
  /**
   * An enumerable object, containing names for the dependency types integer constants.
   * - 0 StartToStart
   * - 1 StartToEnd
   * - 2 EndToStart
   * - 3 EndToEnd
   * @property {Object}
   * @readonly
   * @category Dependency
   */
  static get Type() {
    return {
      StartToStart: 0,
      StartToEnd: 1,
      EndToStart: 2,
      EndToEnd: 3
    };
  }
  static get fields() {
    return [
    // 3 mandatory fields
    /**
     * From event, id of source event
     * @field {String|Number} from
     * @category Dependency
     */
    {
      name: 'from'
    },
    /**
     * To event, id of target event
     * @field {String|Number} to
     * @category Dependency
     */
    {
      name: 'to'
    },
    /**
     * Dependency type, see static property {@link #property-Type-static}
     * @field {Number} type=2
     * @category Dependency
     */
    {
      name: 'type',
      type: 'int',
      defaultValue: 2
    },
    /**
     * CSS class to apply to lines drawn for the dependency
     * @field {String} cls
     * @category Styling
     */
    {
      name: 'cls',
      defaultValue: ''
    },
    /**
     * Bidirectional, drawn with arrows in both directions
     * @field {Boolean} bidirectional
     * @category Dependency
     */
    {
      name: 'bidirectional',
      type: 'boolean'
    },
    /**
     * Start side on source (top, left, bottom, right)
     * @field {'top'|'left'|'bottom'|'right'} fromSide
     * @category Dependency
     */
    {
      name: 'fromSide',
      type: 'string'
    },
    /**
     * End side on target (top, left, bottom, right)
     * @field {'top'|'left'|'bottom'|'right'} toSide
     * @category Dependency
     */
    {
      name: 'toSide',
      type: 'string'
    },
    /**
     * The magnitude of this dependency's lag (the number of units).
     * @field {Number} lag
     * @category Dependency
     */
    {
      name: 'lag',
      type: 'number',
      allowNull: true,
      defaultValue: 0
    },
    /**
     * The units of this dependency's lag, defaults to "d" (days). Valid values are:
     *
     * - "ms" (milliseconds)
     * - "s" (seconds)
     * - "m" (minutes)
     * - "h" (hours)
     * - "d" (days)
     * - "w" (weeks)
     * - "M" (months)
     * - "y" (years)
     *
     * This field is readonly after creation, to change `lagUnit` use {@link #function-setLag setLag()}.
     * @field {'ms'|'s'|'m'|'h'|'d'|'w'|'M'|'y'} lagUnit
     * @category Dependency
     * @readonly
     */
    {
      name: 'lagUnit',
      type: 'string',
      defaultValue: 'd'
    }, {
      name: 'highlighted',
      persist: false,
      internal: true
    }];
  }
  // fromEvent/toEvent defined in CoreDependencyMixin in engine
  /**
   * Gets/sets the source event of the dependency.
   *
   * Accepts multiple formats but always returns an {@link Scheduler.model.EventModel}.
   *
   * **NOTE:** This is not a proper field but rather an alias, it will be serialized but cannot be remapped. If you
   * need to remap, consider using {@link #field-from} instead.
   *
   * @field {Scheduler.model.EventModel} fromEvent
   * @accepts {String|Number|Scheduler.model.EventModel}
   * @category Dependency
   */
  /**
   * Gets/sets the target event of the dependency.
   *
   * Accepts multiple formats but always returns an {@link Scheduler.model.EventModel}.
   *
   * **NOTE:** This is not a proper field but rather an alias, it will be serialized but cannot be remapped. If you
   * need to remap, consider using {@link #field-to} instead.
   *
   * @field {Scheduler.model.EventModel} toEvent
   * @accepts {String|Number|Scheduler.model.EventModel}
   * @category Dependency
   */
  //endregion
  //region Init
  construct(data) {
    const from = data[this.fieldMap.from.dataSource],
      to = data[this.fieldMap.to.dataSource];
    // Engine expects fromEvent and toEvent, not from and to. We need to support both
    if (from != null) {
      data.fromEvent = from;
    }
    if (to != null) {
      data.toEvent = to;
    }
    super.construct(...arguments);
  }
  //endregion
  get eventStore() {
    var _this$unjoinedStores$;
    return this.eventStore || ((_this$unjoinedStores$ = this.unjoinedStores[0]) === null || _this$unjoinedStores$ === void 0 ? void 0 : _this$unjoinedStores$.eventStore);
  }
  set from(value) {
    const {
      fromEvent
    } = this;
    // When assigning a new id to an event, it will update the eventId of the assignment. But the assignments
    // event is still the same so we need to announce here
    if (fromEvent !== null && fromEvent !== void 0 && fromEvent.isModel && fromEvent.id === value) {
      this.set('from', value);
    } else {
      this.fromEvent = value;
    }
  }
  get from() {
    return this.get('from');
  }
  set to(value) {
    const {
      toEvent
    } = this;
    // When assigning a new id to an event, it will update the eventId of the assignment. But the assignments
    // event is still the same so we need to announce here
    if (toEvent !== null && toEvent !== void 0 && toEvent.isModel && toEvent.id === value) {
      this.set('to', value);
    } else {
      this.toEvent = value;
    }
  }
  get to() {
    return this.get('to');
  }
  /**
   * Alias to dependency type, but when set resets {@link #field-fromSide} & {@link #field-toSide} to null as well.
   *
   * @property {Number}
   * @category Dependency
   */
  get hardType() {
    return this.getHardType();
  }
  set hardType(type) {
    this.setHardType(type);
  }
  /**
   * Returns dependency hard type, see {@link #property-hardType}.
   *
   * @returns {Number}
   * @category Dependency
   */
  getHardType() {
    return this.get('type');
  }
  /**
   * Sets dependency {@link #field-type} and resets {@link #field-fromSide} and {@link #field-toSide} to null.
   *
   * @param {Number} type
   * @category Dependency
   */
  setHardType(type) {
    let result;
    if (type !== this.hardType) {
      result = this.set({
        type,
        fromSide: null,
        toSide: null
      });
    }
    return result;
  }
  get lag() {
    return this.get('lag');
  }
  set lag(lag) {
    this.setLag(lag);
  }
  /**
   * Sets lag and lagUnit in one go. Only allowed way to change lagUnit, the lagUnit field is readonly after creation
   * @param {Number|String|Object} lag The lag value. May be just a numeric magnitude, or a full string descriptor eg '1d'
   * @param {'ms'|'s'|'m'|'h'|'d'|'w'|'M'|'y'} [lagUnit] Unit for numeric lag value, see
   * {@link #field-lagUnit} for valid values
   * @category Dependency
   */
  setLag(lag, lagUnit = this.lagUnit) {
    // Either they're only setting the magnitude
    // or, if it's a string, parse the full duration.
    if (arguments.length === 1) {
      if (typeof lag === 'number') {
        this.lag = lag;
      } else {
        lag = DateHelper.parseDuration(lag);
        this.set({
          lag: lag.magnitude,
          lagUnit: lag.unit
        });
      }
      return;
    }
    // Must be a number
    lag = parseFloat(lag);
    this.set({
      lag,
      lagUnit
    });
  }
  getLag() {
    if (this.lag) {
      return `${this.lag < 0 ? '-' : '+'}${Math.abs(this.lag)}${DateHelper.getShortNameOfUnit(this.lagUnit)}`;
    }
    return '';
  }
  /**
   * Property which encapsulates the lag's magnitude and units. An object which contains two properties:
   * @property {Core.data.Duration}
   * @property {Number} fullLag.magnitude The magnitude of the duration
   * @property {'ms'|'s'|'m'|'h'|'d'|'w'|'M'|'y'} fullLag.unit The unit in which the duration is measured, eg
   * `'d'` for days
   * @category Dependency
   */
  get fullLag() {
    return new Duration({
      unit: this.lagUnit,
      magnitude: this.lag
    });
  }
  set fullLag(lag) {
    if (typeof lag === 'string') {
      this.setLag(lag);
    } else {
      this.setLag(lag.magnitude, lag.unit);
    }
  }
  /**
   * Returns true if the linked events have been persisted (e.g. neither of them are 'phantoms')
   *
   * @property {Boolean}
   * @readonly
   * @category Editing
   */
  get isPersistable() {
    const me = this,
      {
        stores,
        unjoinedStores
      } = me,
      store = stores[0];
    let result;
    if (store) {
      const {
          fromEvent,
          toEvent
        } = me,
        crudManager = store.crudManager;
      // if crud manager is used it can deal with phantom source/target since it persists all records in one batch
      // if no crud manager used we have to wait till source/target are persisted
      result = fromEvent && (crudManager || !fromEvent.hasGeneratedId) && toEvent && (crudManager || !toEvent.hasGeneratedId);
    } else {
      result = Boolean(unjoinedStores[0]);
    }
    return result && super.isPersistable;
  }
  getDateRange() {
    const {
      fromEvent,
      toEvent
    } = this;
    if (fromEvent !== null && fromEvent !== void 0 && fromEvent.isScheduled && toEvent !== null && toEvent !== void 0 && toEvent.isScheduled) {
      const Type = DependencyBaseModel.Type;
      let sourceDate, targetDate;
      switch (this.type) {
        case Type.StartToStart:
          sourceDate = fromEvent.startDateMS;
          targetDate = toEvent.startDateMS;
          break;
        case Type.StartToEnd:
          sourceDate = fromEvent.startDateMS;
          targetDate = toEvent.endDateMS;
          break;
        case Type.EndToEnd:
          sourceDate = fromEvent.endDateMS;
          targetDate = toEvent.endDateMS;
          break;
        case Type.EndToStart:
          sourceDate = fromEvent.endDateMS;
          targetDate = toEvent.startDateMS;
          break;
        default:
          throw new Error('Invalid dependency type: ' + this.type);
      }
      return {
        start: Math.min(sourceDate, targetDate),
        end: Math.max(sourceDate, targetDate)
      };
    }
    return null;
  }
  /**
   * Applies given CSS class to dependency, the value doesn't persist
   *
   * @param {String} cls
   * @category Dependency
   */
  highlight(cls) {
    var _this$highlighted;
    const classes = ((_this$highlighted = this.highlighted) === null || _this$highlighted === void 0 ? void 0 : _this$highlighted.split(' ')) ?? [];
    if (!classes.includes(cls)) {
      this.highlighted = classes.concat(cls).join(' ');
    }
  }
  /**
   * Removes given CSS class from dependency if applied, the value doesn't persist
   *
   * @param {String} cls
   * @category Dependency
   */
  unhighlight(cls) {
    const {
      highlighted
    } = this;
    if (highlighted) {
      const classes = highlighted.split(' '),
        index = classes.indexOf(cls);
      if (index >= 0) {
        classes.splice(index, 1);
        this.highlighted = classes.join(' ');
      }
    }
  }
  /**
   * Checks if the given CSS class is applied to dependency.
   *
   * @param {String} cls
   * @returns {Boolean}
   * @category Dependency
   */
  isHighlightedWith(cls) {
    return this.highlighted && this.highlighted.split(' ').includes(cls);
  }
  getConnectorString(raw) {
    const rawValue = canonicalDependencyTypes[this.type];
    if (raw) {
      return rawValue;
    }
    // FS => empty string; it's the default
    if (this.type === DependencyBaseModel.Type.EndToStart) {
      return '';
    }
    return rawValue;
  }
  // getConnectorStringFromType(type, raw) {
  //     const rawValue = canonicalDependencyTypes[type];
  //
  //     if (raw) {
  //         return rawValue;
  //     }
  //
  //     // FS => empty string; it's the default
  //     if (type === DependencyBaseModel.Type.EndToStart) {
  //         return '';
  //     }
  //
  //     const locale = LocaleManager.locale;
  //
  //     // See if there is a local version of SS, SF or FF
  //     if (locale) {
  //         const localized = locale.Scheduler && locale.Scheduler[rawValue];
  //         if (localized) {
  //             return localized;
  //         }
  //     }
  //
  //     return rawValue;
  // }
  // getConnectorString(raw) {
  //     return this.getConnectorStringFromType(this.type);
  // }
  // * getConnectorStringGenerator(raw) {
  //     return this.getConnectorStringFromType(yield this.$.type);
  // }
  toString() {
    return `${this.from}${this.getConnectorString()}${this.getLag()}`;
  }
  /**
   * Returns `true` if the dependency is valid. It is considered valid if it has a valid type and both from and to
   * events are set and pointing to different events.
   *
   * @property {Boolean}
   * @typings ignore
   * @category Editing
   */
  get isValid() {
    const {
      fromEvent,
      toEvent,
      type
    } = this;
    return typeof type === 'number' && fromEvent && toEvent && fromEvent !== toEvent;
  }
  get fromEventName() {
    var _this$fromEvent;
    return ((_this$fromEvent = this.fromEvent) === null || _this$fromEvent === void 0 ? void 0 : _this$fromEvent.name) || '';
  }
  get toEventName() {
    var _this$toEvent;
    return ((_this$toEvent = this.toEvent) === null || _this$toEvent === void 0 ? void 0 : _this$toEvent.name) || '';
  }
  //region STM hooks
  shouldRecordFieldChange(fieldName, oldValue, newValue) {
    if (!super.shouldRecordFieldChange(fieldName, oldValue, newValue)) {
      return false;
    }
    if (fieldName === 'from' || fieldName === 'to' || fieldName === 'fromEvent' || fieldName === 'toEvent') {
      var _this$project;
      const eventStore = (_this$project = this.project) === null || _this$project === void 0 ? void 0 : _this$project.eventStore;
      if (eventStore && eventStore.oldIdMap[oldValue] === eventStore.getById(newValue)) {
        return false;
      }
    }
    return true;
  }
  //endregion
}

DependencyBaseModel.exposeProperties();
DependencyBaseModel._$name = 'DependencyBaseModel';

const EngineMixin$2 = CoreDependencyMixin;
/**
 * @module Scheduler/model/DependencyModel
 */
/**
 * This model represents a dependency between two events, usually added to a {@link Scheduler.data.DependencyStore}.
 *
 * It is a subclass of the {@link Scheduler.model.DependencyBaseModel} class, which in its turn subclasses
 * {@link Core.data.Model}. Please refer to documentation of those classes to become familiar with the base interface of
 * this class.
 *
 * ## Fields and references
 *
 * A Dependency has a few predefined fields, see Fields below.  The name of any fields data source can be customized in
 * the subclass, see the example below. Please also refer to {@link Core.data.Model} for details.
 *
 * ```javascript
 * class MyDependency extends DependencyModel {
 *   static get fields() {
 *     return [
 *       { name: 'to', dataSource: 'targetId' },
 *       { name: 'from', dataSource: 'sourceId' }
 *     ]);
 *   }
 * }
 * ```
 *
 * After load and project normalization, these references are accessible (assuming their respective stores are loaded):
 * - `fromEvent` - The event on the start side of the dependency
 * - `toEvent` - The event on the end side of the dependency
 *
 * ## Async resolving of references
 *
 * As described above, a dependency has links to events. These references are populated async, using the calculation
 * engine of the project that the resource via its store is a part of. Because of this asyncness, references cannot be
 * used immediately after modifications:
 *
 * ```javascript
 * dependency.from = 2;
 * // dependency.fromEvent is not yet up to date
 * ```
 *
 * To make sure references are updated, wait for calculations to finish:
 *
 * ```javascript
 * dependency.from = 2;
 * await dependency.project.commitAsync();
 * // dependency.fromEvent is up to date
 * ```
 *
 * As an alternative, you can also use `setAsync()` to trigger calculations directly after the change:
 *
 * ```javascript
 * await dependency.setAsync({ from : 2});
 * // dependency.fromEvent is up to date
 * ```
 *
 * @extends Scheduler/model/DependencyBaseModel
 * @uninherit Core/data/mixin/TreeNode
 */
class DependencyModel extends PartOfProject(EngineMixin$2.derive(DependencyBaseModel)) {
  static get $name() {
    return 'DependencyModel';
  }
  // Determines the type of dependency based on fromSide and toSide
  getTypeFromSides(fromSide, toSide, rtl) {
    const types = DependencyBaseModel.Type,
      startSide = rtl ? 'right' : 'left',
      endSide = rtl ? 'left' : 'right';
    if (fromSide === startSide) {
      return toSide === startSide ? types.StartToStart : types.StartToEnd;
    }
    return toSide === endSide ? types.EndToEnd : types.EndToStart;
  }
}
DependencyModel.exposeProperties();
DependencyModel._$name = 'DependencyModel';

/**
 * @module Scheduler/data/mixin/DependencyStoreMixin
 */
/**
 * This is a mixin, containing functionality related to managing dependencies.
 *
 * It is consumed by the regular {@link Scheduler.data.DependencyStore} class and Scheduler Pros counterpart.
 *
 * @mixin
 */
var DependencyStoreMixin = (Target => class DependencyStoreMixin extends Target {
  static get $name() {
    return 'DependencyStoreMixin';
  }
  /**
   * Add dependencies to the store.
   *
   * NOTE: References (fromEvent, toEvent) on the dependencies are determined async by a calculation engine. Thus they
   * cannot be directly accessed after using this function.
   *
   * For example:
   *
   * ```javascript
   * const [dependency] = dependencyStore.add({ from, to });
   * // dependency.fromEvent is not yet available
   * ```
   *
   * To guarantee references are set up, wait for calculations for finish:
   *
   * ```javascript
   * const [dependency] = dependencyStore.add({ from, to });
   * await dependencyStore.project.commitAsync();
   * // dependency.fromEvent is available (assuming EventStore is loaded and so on)
   * ```
   *
   * Alternatively use `addAsync()` instead:
   *
   * ```javascript
   * const [dependency] = await dependencyStore.addAsync({ from, to });
   * // dependency.fromEvent is available (assuming EventStore is loaded and so on)
   * ```
   *
   * @param {Scheduler.model.DependencyModel|Scheduler.model.DependencyModel[]|DependencyModelConfig|DependencyModelConfig[]} records
   * Array of records/data or a single record/data to add to store
   * @param {Boolean} [silent] Specify `true` to suppress events
   * @returns {Scheduler.model.DependencyModel[]} Added records
   * @function add
   * @category CRUD
   */
  /**
   * Add dependencies to the store and triggers calculations directly after. Await this function to have up to date
   * references on the added dependencies.
   *
   * ```javascript
   * const [dependency] = await dependencyStore.addAsync({ from, to });
   * // dependency.fromEvent is available (assuming EventStore is loaded and so on)
   * ```
   *
   * @param {Scheduler.model.DependencyModel|Scheduler.model.DependencyModel[]|DependencyModelConfig|DependencyModelConfig[]} records
   * Array of records/data or a single record/data to add to store
   * @param {Boolean} [silent] Specify `true` to suppress events
   * @returns {Scheduler.model.DependencyModel[]} Added records
   * @function addAsync
   * @category CRUD
   * @async
   */
  /**
   * Applies a new dataset to the DependencyStore. Use it to plug externally fetched data into the store.
   *
   * NOTE: References (fromEvent, toEvent) on the dependencies are determined async by a calculation engine. Thus
   * they cannot be directly accessed after assigning the new dataset.
   *
   * For example:
   *
   * ```javascript
   * dependencyStore.data = [{ from, to }];
   * // dependencyStore.first.fromEvent is not yet available
   * ```
   *
   * To guarantee references are available, wait for calculations for finish:
   *
   * ```javascript
   * dependencyStore.data = [{ from, to }];
   * await dependencyStore.project.commitAsync();
   * // dependencyStore.first.fromEvent is available
   * ```
   *
   * Alternatively use `loadDataAsync()` instead:
   *
   * ```javascript
   * await dependencyStore.loadDataAsync([{ from, to }]);
   * // dependencyStore.first.fromEvent is available
   * ```
   *
   * @member {DependencyModelConfig[]} data
   * @category Records
   */
  /**
   * Applies a new dataset to the DependencyStore and triggers calculations directly after. Use it to plug externally
   * fetched data into the store.
   *
   * ```javascript
   * await dependencyStore.loadDataAsync([{ from, to }]);
   * // dependencyStore.first.fromEvent is available
   * ```
   *
   * @param {DependencyModelConfig[]} data Array of DependencyModel data objects
   * @function loadDataAsync
   * @category CRUD
   * @async
   */
  static get defaultConfig() {
    return {
      /**
       * CrudManager must load stores in the correct order. Lowest first.
       * @private
       */
      loadPriority: 400,
      /**
       * CrudManager must sync stores in the correct order. Lowest first.
       * @private
       */
      syncPriority: 400,
      storeId: 'dependencies'
    };
  }
  reduceEventDependencies(event, reduceFn, result, flat = true, depsGetterFn) {
    depsGetterFn = depsGetterFn || (event => this.getEventDependencies(event));
    event = ArrayHelper.asArray(event);
    event.reduce((result, event) => {
      if (event.children && !flat) {
        event.traverse(evt => {
          result = depsGetterFn(evt).reduce(reduceFn, result);
        });
      } else {
        result = depsGetterFn(event).reduce(reduceFn, result);
      }
    }, result);
    return result;
  }
  mapEventDependencies(event, fn, filterFn, flat, depsGetterFn) {
    return this.reduceEventDependencies(event, (result, dependency) => {
      filterFn(dependency) && result.push(dependency);
      return result;
    }, [], flat, depsGetterFn);
  }
  mapEventPredecessors(event, fn, filterFn, flat) {
    return this.reduceEventPredecessors(event, (result, dependency) => {
      filterFn(dependency) && result.push(dependency);
      return result;
    }, [], flat);
  }
  mapEventSuccessors(event, fn, filterFn, flat) {
    return this.reduceEventSuccessors(event, (result, dependency) => {
      filterFn(dependency) && result.push(dependency);
      return result;
    }, [], flat);
  }
  /**
   * Returns all dependencies for a certain event (both incoming and outgoing)
   *
   * @param {Scheduler.model.EventModel} event
   * @returns {Scheduler.model.DependencyModel[]}
   */
  getEventDependencies(event) {
    return [].concat(event.predecessors || [], event.successors || []);
  }
  removeEventDependencies(event) {
    this.remove(this.getEventDependencies(event));
  }
  removeEventPredecessors(event) {
    this.remove(event.predecessors);
  }
  removeEventSuccessors(event, flat) {
    this.remove(event.successors);
  }
  getBySourceTargetId(key) {
    return this.records.find(r => key == this.constructor.makeDependencySourceTargetCompositeKey(r.from, r.to));
  }
  /**
   * Returns dependency model instance linking tasks with given ids. The dependency can be forward (from 1st
   * task to 2nd) or backward (from 2nd to 1st).
   *
   * @param {Scheduler.model.EventModel|String} sourceEvent 1st event
   * @param {Scheduler.model.EventModel|String} targetEvent 2nd event
   * @returns {Scheduler.model.DependencyModel}
   */
  getDependencyForSourceAndTargetEvents(sourceEvent, targetEvent) {
    sourceEvent = Model.asId(sourceEvent);
    targetEvent = Model.asId(targetEvent);
    return this.getBySourceTargetId(this.constructor.makeDependencySourceTargetCompositeKey(sourceEvent, targetEvent));
  }
  /**
   * Returns a dependency model instance linking given events if such dependency exists in the store.
   * The dependency can be forward (from 1st event to 2nd) or backward (from 2nd to 1st).
   *
   * @param {Scheduler.model.EventModel|String} sourceEvent
   * @param {Scheduler.model.EventModel|String} targetEvent
   * @returns {Scheduler.model.DependencyModel}
   */
  getEventsLinkingDependency(sourceEvent, targetEvent) {
    return this.getDependencyForSourceAndTargetEvents(sourceEvent, targetEvent) || this.getDependencyForSourceAndTargetEvents(targetEvent, sourceEvent);
  }
  /**
   * Validation method used to validate a dependency. Override and return `true` to indicate that an
   * existing dependency between two tasks is valid. For a new dependency being created please see
   * {@link #function-isValidDependencyToCreate}.
   *
   * @param {Scheduler.model.DependencyModel|Scheduler.model.TimeSpan|Number|String} dependencyOrFromId The dependency
   * model, the from task/event or the id of the from task/event
   * @param {Scheduler.model.TimeSpan|Number|String} [toId] To task/event or id thereof if the first parameter is not
   * a dependency record
   * @param {Number} [type] Dependency {@link Scheduler.model.DependencyBaseModel#property-Type-static} if the first
   * parameter is not a dependency model instance.
   * @returns {Boolean}
   */
  async isValidDependency(dependencyOrFromId, toId, type) {
    let fromEvent = dependencyOrFromId,
      toEvent = toId;
    if (dependencyOrFromId == null) {
      return false;
    }
    // Accept dependency model
    if (dependencyOrFromId.isDependencyModel) {
      ({
        fromEvent,
        toEvent
      } = dependencyOrFromId);
    }
    // Accept from as id
    fromEvent = this.eventStore.getById(fromEvent);
    // Accept to as id
    toEvent = this.eventStore.getById(toEvent);
    // This condition is supposed to map all model instances to be validated by project. Lowest common ancestor
    // for scheduler event, scheduler pro event and gantt task is TimeSpan
    if (fromEvent && toEvent) {
      // Block creating dependencies to display only tasks in Gantt
      if (!fromEvent.project || !toEvent.project) {
        return false;
      }
      // Not asserting dependency type here. Default value should normally suffice.
      return this.project.isValidDependency(fromEvent, toEvent, type);
    }
    return dependencyOrFromId !== toId;
  }
  /**
   * Validation method used to validate a dependency while creating. Override and return `true` to indicate that
   * a new dependency is valid to be created.
   *
   * @param {Scheduler.model.TimeSpan|Number|String} fromId From event/task or id
   * @param {Scheduler.model.TimeSpan|Number|String} toId To event/task or id
   * @param {Number} type Dependency {@link Scheduler.model.DependencyBaseModel#property-Type-static}
   * @returns {Boolean}
   */
  isValidDependencyToCreate(fromId, toId, type) {
    return this.isValidDependency(fromId, toId, type);
  }
  /**
   * Returns all dependencies highlighted with the given CSS class
   *
   * @param {String} cls
   * @returns {Scheduler.model.DependencyBaseModel[]}
   */
  getHighlightedDependencies(cls) {
    return this.records.reduce((result, dep) => {
      if (dep.isHighlightedWith(cls)) result.push(dep);
      return result;
    }, []);
  }
  static makeDependencySourceTargetCompositeKey(from, to) {
    return `source(${from})-target(${to})`;
  }
  //region Product neutral
  getTimeSpanDependencies(record) {
    return this.getEventDependencies(record);
  }
  //endregion
});

const EngineMixin$1 = PartOfProject(CoreDependencyStoreMixin.derive(AjaxStore));
/**
 * @module Scheduler/data/DependencyStore
 */
/**
 * A store representing a collection of dependencies between events in the {@link Scheduler.data.EventStore}.
 *
 * This store only accepts a model class inheriting from {@link Scheduler.model.DependencyModel}.
 *
 * A DependencyStore is usually connected to a project, which binds it to other related stores (EventStore,
 * AssignmentStore and ResourceStore). The project also handles references (fromEvent, toEvent) to related records
 * for the records in the store.
 *
 * Resolving the references happens async, records are not guaranteed to have up to date references until calculations
 * are finished. To be certain that references are resolved, call `await project.commitAsync()` after store actions. Or
 * use one of the `xxAsync` functions, such as `loadDataAsync()`.
 *
 * Using `commitAsync()`:
 *
 * ```javascript
 * dependencyStore.data = [{ from, to }, ...];
 *
 * // references (fromEvent, toEvent) not resolved yet
 *
 * await dependencyStore.project.commitAsync();
 *
 * // now they are
 * ```
 *
 * Using `loadDataAsync()`:
 *
 * ```javascript
 * await dependencyStore.loadDataAsync([{ from, to }, ...]);
 *
 * // references (fromEvent, toEvent) are resolved
 * ```
 *
 * @mixes Scheduler/data/mixin/PartOfProject
 * @mixes Scheduler/data/mixin/DependencyStoreMixin
 * @extends Core/data/AjaxStore
 */
class DependencyStore extends DependencyStoreMixin(EngineMixin$1.derive(AjaxStore)) {
  static get defaultConfig() {
    return {
      modelClass: DependencyModel
    };
  }
}
DependencyStore._$name = 'DependencyStore';

/**
 * @module Scheduler/data/mixin/ProjectCrudManager
 */
/**
 * This mixin provides Crud Manager functionality supporting loading of scheduling engine projects.
 *
 * @mixin
 * @mixes Scheduler/crud/AbstractCrudManagerMixin
 * @mixes Scheduler/crud/transport/AjaxTransport
 * @mixes Scheduler/crud/encoder/JsonEncoder
 */
var ProjectCrudManager = (Target => class ProjectCrudManager extends (Target || Base).mixin(AbstractCrudManagerMixin, AjaxTransport, JsonEncoder) {
  //region Config
  static get defaultConfig() {
    return {
      project: null
    };
  }
  startConfigure(config) {
    // process the project first which ingests any configured data sources,
    this.getConfig('project');
    super.startConfigure(config);
    this._changesToClear = new Map();
  }
  async doAutoLoad() {
    const {
      project
    } = this;
    // Delay autoLoad to after projects initial commit if configured with a project
    if (project) {
      await project.commitAsync();
    }
    return super.doAutoLoad();
  }
  applyProjectResponse(response) {
    const me = this,
      {
        project
      } = me;
    me.applyingProjectResponse = true;
    const startDateField = project.fieldMap.startDate,
      endDateField = project.fieldMap.endDate,
      startDate = ObjectHelper.getPath(response, startDateField.dataSource),
      endDate = ObjectHelper.getPath(response, endDateField.dataSource);
    // With early rendering engine won't convert project dates in time, do it manually first
    if (typeof startDate === 'string') {
      ObjectHelper.setPath(response, startDateField.dataSource, startDateField.convert(startDate));
    }
    if (typeof endDate === 'string') {
      ObjectHelper.setPath(response, endDateField.dataSource, endDateField.convert(endDate));
    }
    Object.assign(project, response);
    me._changesToClear.set(me, response);
    me.applyingProjectResponse = false;
  }
  loadCrudManagerData(response, options = {}) {
    const me = this,
      {
        project
      } = me;
    // We don't want to react on store changes while loading them
    me.suspendChangesTracking();
    super.loadCrudManagerData(...arguments);
    // If there is project data provided
    if (response !== null && response !== void 0 && response.project) {
      // If the project is delaying its linking to a new graph instance
      // wait till it detaches all records from the old graph
      // and then apply the project data.
      // Otherwise the project changes we apply here will be overridden when detaching from the old graph.
      // Since the code copies last identifier values from the graph when detaching.
      if (project.delayEnteringReplica) {
        project.ion({
          recordsUnlinked: () => {
            me.suspendChangesTracking();
            me.applyProjectResponse(response.project);
            me.resumeChangesTracking();
          },
          once: true
        });
      } else {
        me.applyProjectResponse(response.project);
      }
    }
    me.resumeChangesTracking();
  }
  async sync() {
    const {
      project
    } = this;
    // Suspend Crud Manager autoSync to not react on changes during commitAsync() call
    this.suspendAutoSync();
    // Make sure data is in a calculated state before syncing
    if (project) {
      await project.commitAsync();
    }
    if (this.isDestroying) {
      return;
    }
    // resume autoSync silently
    this.resumeAutoSync(false);
    return super.sync();
  }
  async applyResponse(request, response, options) {
    var _me$project, _request$pack, _me$trigger;
    const me = this;
    if (me.isDestroyed || (_me$project = me.project) !== null && _me$project !== void 0 && _me$project.isDestroyed) {
      return;
    }
    me.trigger('beforeApplyResponse');
    await super.applyResponse(request, response, options);
    // If there is the project data provided or it's not provided but short responses are enabled
    // in such cases we treat a successful response covering all sent changes as much as that's possible
    // so responding `project : { ... }` should not be really needed
    if (response !== null && response !== void 0 && response.project || me.supportShortSyncResponse && request !== null && request !== void 0 && (_request$pack = request.pack) !== null && _request$pack !== void 0 && _request$pack.project) {
      var _request$pack2;
      // apply the responded changes (or accept the requested ones)
      me.applyProjectResponse(response.project || (request === null || request === void 0 ? void 0 : (_request$pack2 = request.pack) === null || _request$pack2 === void 0 ? void 0 : _request$pack2.project));
    }
    // if we have a project
    if (me.project) {
      var _me$resumeAutoSync, _me$commitRespondedCh;
      let requestType = request.type;
      // response can force its type
      if (me.trackResponseType) {
        requestType = response.type || requestType;
      }
      // Make a boolean flag indicating what has triggered the propagation ("propagatingLoadChanges" or  "propagatingSyncChanges")
      const propagationFlag = `propagating${StringHelper.capitalize(requestType)}Changes`;
      me.suspendAutoSync();
      me[propagationFlag] = true;
      const loud = me.project.isInitialCommit && !me.project.silenceInitialCommit;
      // Wait till calculation gets done
      await me.project.commitAsync();
      me[propagationFlag] = false;
      // for initial commit we still trigger the sync, but not for following
      (_me$resumeAutoSync = me.resumeAutoSync) === null || _me$resumeAutoSync === void 0 ? void 0 : _me$resumeAutoSync.call(me, loud);
      // Accept changes came from the server (might have been destroyed while waiting above)
      (_me$commitRespondedCh = me.commitRespondedChanges) === null || _me$commitRespondedCh === void 0 ? void 0 : _me$commitRespondedCh.call(me);
    }
    // Trigger if we did not get destroyed while waiting above
    (_me$trigger = me.trigger) === null || _me$trigger === void 0 ? void 0 : _me$trigger.call(me, 'applyResponse');
  }
  applySyncResponse(...args) {
    var _me$project2;
    const me = this,
      stmDisabled = (_me$project2 = me.project) === null || _me$project2 === void 0 ? void 0 : _me$project2.stm.disabled;
    if (stmDisabled === false && me.ignoreRemoteChangesInSTM) {
      me.project.stm.disable();
    }
    super.applySyncResponse(...args);
    if (stmDisabled === false) {
      me.project.stm.enable();
    }
  }
  shouldClearRecordFieldChange(record, field, value) {
    var _field;
    const oldValue = record[field];
    field = record.getFieldDefinition(field);
    // If the field value matches the one responded from the server
    // we silently accept it
    return (_field = field) !== null && _field !== void 0 && _field.isEqual ? field.isEqual(oldValue, value) : ObjectHelper.isEqual(oldValue, value);
  }
  commitRespondedChanges() {
    // We silently accept changes came from the server
    this._changesToClear.forEach((changes, record) => {
      Object.entries(changes).forEach(([key, value]) => {
        // If the field value matches the one responded from the server
        // we silently accept it
        if (this.shouldClearRecordFieldChange(record, key, value)) {
          delete record.meta.modified[key];
        }
      });
    });
    this._changesToClear.clear();
  }
  applyChangesToStore(storeDesc, storeResponse, storePack, ...rest) {
    const changesMap = super.applyChangesToStore(storeDesc, storeResponse, storePack, ...rest);
    // The changes enter graph first but not store until later, clearing changes in StoreChanges might still leave
    // records modified. We need to clean up after the next propagation, so we store record changes in a map for now
    if (changesMap.size && this.project) {
      for (const [id, changes] of changesMap) {
        const record = storeDesc.store.getById(id);
        record && this._changesToClear.set(record, changes);
      }
    }
    return changesMap;
  }
});

/**
 * @module Scheduler/model/mixin/ProjectModelCommon
 */
/**
 * Mixin that provides helpful methods and properties for a `ProjectModel`. This mixin applies to all Bryntum products.
 * @mixin
 * @internal
 */
var ProjectModelCommon = (Target => class ProjectModelCommon extends (Target || Model) {
  static $name = 'ProjectModelCommon';
  static get configurable() {
    return {
      // Documented in Gantt/Scheduler/SchedulerPro version of ./model/ProjectModel since types differ
      assignments: null,
      dependencies: null,
      resources: null,
      timeRanges: null
    };
  }
  //region Inline data
  get assignments() {
    return this.assignmentStore.allRecords;
  }
  updateAssignments(assignments) {
    this.assignmentStore.data = assignments;
  }
  get dependencies() {
    return this.dependencyStore.allRecords;
  }
  updateDependencies(dependencies) {
    this.dependencyStore.data = dependencies;
  }
  get resources() {
    return this.resourceStore.allRecords;
  }
  updateResources(resources) {
    this.resourceStore.data = resources;
  }
  get timeRanges() {
    return this.timeRangeStore.allRecords;
  }
  getTimeRanges(startDate, endDate) {
    const store = this.timeRangeStore,
      ret = [];
    for (const timeSpan of store) {
      // Collect occurrences for the recurring events in the record set
      if (timeSpan.isRecurring) {
        ret.push(...timeSpan.getOccurrencesForDateRange(startDate, endDate));
      } else if (timeSpan.startDate < endDate && startDate < timeSpan.endDate) {
        ret.push(timeSpan);
      }
    }
    return ret;
  }
  updateTimeRanges(timeRanges) {
    this.timeRangeStore.data = timeRanges;
  }
  getResourceTimeRanges(startDate, endDate) {
    const store = this.resourceTimeRangeStore,
      ret = [];
    for (const timeSpan of store) {
      // Collect occurrences for the recurring events in the record set
      if (timeSpan.isRecurring) {
        ret.push(...timeSpan.getOccurrencesForDateRange(startDate, endDate));
      } else if (timeSpan.startDate < endDate && startDate < timeSpan.endDate) {
        ret.push(timeSpan);
      }
    }
    return ret;
  }
  //endregion
});

/**
 * @module Scheduler/model/ResourceTimeRangeModel
 */
/**
 * This class represent a single resource time range in your schedule.
 * To style the rendered elements, use {@link Scheduler.model.TimeSpan#field-cls} or {@link #field-timeRangeColor} field.
 * The class is used by the {@link Scheduler.feature.ResourceTimeRanges} feature.
 *
 * ## Recurring ranges support
 *
 * You can also make ranges recurring by adding a `recurrenceRule` to the range data.
 *
 * ```javascript
 *
 * // Make new store that supports time ranges recurrence
 * const store = new ResourceTimeRangeStore({
 *     data : [{        {
 *         id             : 1,
 *         resourceId     : 'r1',
 *         startDate      : '2019-01-01T11:00',
 *         endDate        : '2019-01-01T13:00',
 *         name           : 'Coffee break',
 *         // this time range should repeat every day
 *         recurrenceRule : 'FREQ=DAILY'
 *     }]
 * });
 * ```
 *
 * @extends Scheduler/model/TimeSpan
 */
class ResourceTimeRangeModel extends TimeSpan.mixin(RecurringTimeSpan) {
  static $name = 'ResourceTimeRangeModel';
  //region Fields
  static fields = [
  /**
   * Id of the resource this time range is associated with
   * @field {String|Number} resourceId
   */
  'resourceId',
  /**
   * Controls this time ranges primary color, defaults to using current themes default time range color.
   * @field {String} timeRangeColor
   */
  'timeRangeColor'];
  static relations = {
    /**
     * The associated resource, retrieved using a relation to a ResourceStore determined by the value assigned
     * to `resourceId`. The relation also lets you access all time ranges on a resource through
     * `ResourceModel#timeRanges`.
     * @member {Scheduler.model.ResourceModel} resource
     */
    resource: {
      foreignKey: 'resourceId',
      foreignStore: 'resourceStore',
      relatedCollectionName: 'timeRanges',
      nullFieldOnRemove: true
    }
  };
  static domIdPrefix = 'resourcetimerange';
  get domId() {
    return `${this.constructor.domIdPrefix}-${this.id}`;
  }
  //endregion
  // Used internally to differentiate between Event and ResourceTimeRange
  get isResourceTimeRange() {
    return true;
  }
  // To match EventModel API
  get resources() {
    return this.resource ? [this.resource] : [];
  }
  // To match EventModel API
  get $linkedResources() {
    return this.resources;
  }
}
ResourceTimeRangeModel._$name = 'ResourceTimeRangeModel';

/**
 * @module Scheduler/data/ResourceTimeRangeStore
 */
/**
 * A class representing a collection of resource time ranges.
 * Contains a collection of {@link Scheduler.model.ResourceTimeRangeModel ResourceTimeRangeModel} records.
 * The class is used by the {@link Scheduler.feature.ResourceTimeRanges ResourceTimeRanges} feature.
 *
 * ## Recurring ranges support
 *
 * By default, the class doesn't support recurrence. In order to add its support
 * please use {@link Scheduler.data.mixin.RecurringTimeSpansMixin RecurringTimeSpansMixin} mixin
 * (the store model in turn should be mixed with {@link Scheduler/model/mixin/RecurringTimeSpan RecurringTimeSpan} class):
 *
 * ```js
 * // Mix RecurringTimeSpan (which adds recurrence support) to ResourceTimeRangeModel
 * class MyResourceTimeRange extends RecurringTimeSpan(ResourceTimeRangeModel) {};
 *
 * // Mix RecurringTimeSpansMixin (which adds recurrence support) to ResourceTimeRangeStore
 * class MyResourceTimeRangeStore extends RecurringTimeSpansMixin(ResourceTimeRangeStore) {
 *     static get defaultConfig() {
 *         return {
 *             // use our new MyResourceTimeRange model
 *             modelClass : MyResourceTimeRange
 *         };
 *     }
 * };
 *
 * // Make new store that supports time ranges recurrence
 * const store = new MyResourceTimeRangeStore({
 *     data : [{        {
 *         id             : 1,
 *         resourceId     : 'r1',
 *         startDate      : '2019-01-01T11:00',
 *         endDate        : '2019-01-01T13:00',
 *         name           : 'Coffee break',
 *         // this time range should repeat every day
 *         recurrenceRule : 'FREQ=DAILY'
 *     }]
 * });
 * ```
 *
 * @extends Core/data/AjaxStore
 */
class ResourceTimeRangeStore extends AjaxStore.mixin(RecurringTimeSpansMixin) {
  static $name = 'ResourceTimeRangeStore';
  static get defaultConfig() {
    return {
      /**
       * CrudManager must load stores in the correct order. Lowest first.
       * @private
       */
      loadPriority: 500,
      /**
       * CrudManager must sync stores in the correct order. Lowest first.
       * @private
       */
      syncPriority: 500,
      /**
       * This store should be linked to a ResourceStore to link the time ranges to resources
       * @config {Scheduler.data.ResourceStore}
       */
      resourceStore: null,
      modelClass: ResourceTimeRangeModel,
      storeId: 'resourceTimeRanges'
    };
  }
  set resourceStore(store) {
    this._resourceStore = store;
    // If store is assigned after configuration we need to init relations
    if (!this.isConfiguring) {
      this.initRelations(true);
    }
  }
  get resourceStore() {
    return this._resourceStore;
  }
  // Matching signature in EventStore to allow reusage of SchedulerStores#onInternalEventStoreChange()
  getResourcesForEvent(resourceTimeRange) {
    return [resourceTimeRange.resource];
  }
  /**
   * Get resource time ranges intersecting the specified date range for a resource.
   *
   * The result is sorted by `startDate`.
   *
   * @param {Object} options Options
   * @param {Scheduler.model.ResourceModel} options.resourceRecord Resource record
   * @param {Date} options.startDate Start date of the range
   * @param {Date} options.endDate End date of the range
   * @returns {Scheduler.model.ResourceTimeRangeModel[]}
   */
  getRanges({
    resourceRecord,
    startDate,
    endDate
  }) {
    const rangesInDateRange = resourceRecord.timeRanges.flatMap(range => {
      if (range.supportsRecurring) {
        return range.getOccurrencesForDateRange(startDate, endDate);
      }
      if (range.intersectsRange(startDate, endDate)) {
        return range;
      }
      return [];
    });
    return rangesInDateRange.sort((span1, span2) => span1.startDate - span2.startDate);
  }
}
ResourceTimeRangeStore._$name = 'ResourceTimeRangeStore';

/**
 * @module Scheduler/model/mixin/ProjectModelTimeZoneMixin
 */
/**
 * Mixin that holds TimeZone functionality shared between projects in Scheduler, Scheduler Pro, Gantt and Calendar
 * @mixin
 */
var ProjectModelTimeZoneMixin = (Target => class ProjectModelTimeZoneMixin extends (Target || Model) {
  static $name = 'ProjectModelTimeZoneMixin';
  static configurable = {
    /**
     * Set to a IANA time zone (i.e. `Europe/Stockholm`) or a UTC offset in minutes (i.e. `-120`). This will
     * convert all events, tasks and time ranges to the specified time zone or offset. It will also affect the
     * displayed timeline's headers as well at the start and end date of it.
     *
     * There is currently no built-in time zone support in JavaScript which means that the converted dates
     * technically still are in the local system time zone, but adjusted to match the configured time zone.
     *
     * ### DST
     * If a IANA time zone is provided, there will be support for DST. But if local system time zone has DST that
     * will affect the time zone conversion at the exact hour when the local system time zone switches DST on and
     * off.
     *
     * *For example:*
     * 1. The local system time zone is `Europe/Stockholm` (which is UTC+1 or UTC+2 when DST).
     * 2. The date `2022-03-27T07:00:00Z` (which is UTC) is converted to `America/Chicago` (which is UTC-6 or UTC-5
     *    when DST).
     * 3. The converted JS date will be created from `2022-03-27T02:00:00` which is exactly the hour when
     *    `Europe/Stockholm` adds an DST hour. This has the effect that the converted date shows up incorrectly as
     *    `2022-03-27T03:00` instead.
     *
     * If a UTC offset is provided, there is no DST support at all.
     *
     * ### Editing
     * If creating new records or editing existing record dates, the dates will be interpreted as in the selected
     * time zone.
     *
     * ### Saving
     * When saving or syncing data, the dates will be restored to local system time and converted to JSON
     * ISO formatted.
     *
     * @prp {String|Number} [timeZone]
     * @category Advanced
     */
    timeZone: {
      // Don't ingest the config eagerly because it relies on project being present.
      // Lazy means it waits for ingestion until timeZone property is referenced.
      $config: 'lazy',
      value: null
    }
  };
  get _storesWithDates() {
    return [this.taskStore, this.timeRangeStore, this.resourceTimeRangeStore].filter(s => s);
  }
  // Overrides a Store's processRecord function to be able to convert records added by a dataset
  // before they are processed by the engine
  overrideStoreProcessRecord(store) {
    if (this._storesWithDates.includes(store) && !store.$originalProcessRecord) {
      store.$originalProcessRecord = FunctionHelper.before(store, 'processRecord', (record, isDataSet) => {
        if (isDataSet || store.isLoadingData) {
          // When loading or changing dataset, de data is treated as local dates and need to be converted
          this.convertRecordToTimeZone(record);
        }
      });
    }
  }
  attachStore(store) {
    super.attachStore(store);
    // If a new store is attached, convert it to the configured timezone
    if (store && this.timeZone != null && this._storesWithDates.includes(store)) {
      this.convertStoresToTimeZone([store]);
      this.overrideStoreProcessRecord();
    }
  }
  detachStore(store) {
    super.detachStore(store);
    // When a store is detached, the records isn't timezone converted anymore
    if (store && !store.isDestroyed) {
      const {
        $originalProcessRecord
      } = store;
      if ($originalProcessRecord) {
        $originalProcessRecord(); // Restores the original function
        store.$originalProcessRecord = null;
        // Convert records back to local system timezone
        this.convertStoresToTimeZone([store], null);
      }
    }
  }
  relayStoreChange({
    source,
    action,
    records,
    replaced
  }) {
    const me = this;
    if (me.timeZone != null && me._storesWithDates.includes(source)) {
      // When a record is added or replaced, it is treated as in current configured timezone
      if (['add', 'replace'].includes(action)) {
        var _records;
        if (!((_records = records) !== null && _records !== void 0 && _records.length) && replaced !== null && replaced !== void 0 && replaced.length) {
          records = replaced;
        }
        if (records.length) {
          records.forEach(record => record.timeZone = me.timeZone);
        }
      }
    }
  }
  convertStoresToTimeZone(stores, timeZone = this.timeZone) {
    var _me$stm;
    const me = this,
      stmAutoRecord = (_me$stm = me.stm) === null || _me$stm === void 0 ? void 0 : _me$stm.autoRecord;
    // Disable STM while updating timezone values
    if (stmAutoRecord) {
      me.stm.autoRecord = false;
    }
    for (const store of stores) {
      store === null || store === void 0 ? void 0 : store.forEach(r => me.convertRecordToTimeZone(r, timeZone));
    }
    if (stmAutoRecord) {
      // Restore original value
      me.stmAutoRecord = stmAutoRecord;
    }
  }
  convertRecordToTimeZone(record, timeZone = this.timeZone) {
    if (record.timeZone !== timeZone) {
      var _record$baselines;
      record.$ignoreChange = true; // Used to ignore changes in NestedEvents feature
      // Convert baselines if exists
      if ((_record$baselines = record.baselines) !== null && _record$baselines !== void 0 && _record$baselines.count) {
        for (const bl of record.baselines) {
          // The baseline records is not marked with a timezone when they are created
          if (record.timeZone !== bl.timeZone) {
            bl.timeZone = record.timeZone;
          }
          bl.convertToTimeZone(timeZone);
        }
      }
      record.convertToTimeZone(timeZone);
      record.$ignoreChange = false;
    }
  }
  updateTimeZone(timeZone, oldTimeZone) {
    const me = this,
      isConfiguring = me._isConfiguringTimeZone || me.isConfiguring;
    me.trigger('beforeTimeZoneChange', {
      timeZone,
      oldTimeZone,
      isConfiguring
    });
    me.calendarManagerStore.forEach(calendar => calendar.bumpVersion());
    // Convert data to time zone
    me.convertStoresToTimeZone(me._storesWithDates);
    me._storesWithDates.forEach(store => me.overrideStoreProcessRecord(store));
    // Convert project startdate
    if (me.startDate) {
      const startDate = oldTimeZone != null ? TimeZoneHelper.fromTimeZone(me.startDate, oldTimeZone) : me.startDate;
      me.startDate = timeZone != null ? TimeZoneHelper.toTimeZone(startDate, timeZone) : startDate;
    }
    me.ignoreRecordChanges = true;
    me.commitAsync().then(() => {
      if (!me.isDestroyed) {
        me.trigger('timeZoneChange', {
          timeZone,
          oldTimeZone,
          isConfiguring
        });
      }
      delete me._isConfiguringTimeZone;
    });
  }
});

/**
 * @module Scheduler/model/mixin/ProjectModelMixin
 */
/**
 * Mixin that holds configuration shared between projects in Scheduler and Scheduler Pro.
 * @mixes Scheduler/model/mixin/ProjectModelTimeZoneMixin
 * @mixin
 */
var ProjectModelMixin = (Target => class ProjectModelMixin extends (Target || Model).mixin(ProjectModelCommon, ProjectModelTimeZoneMixin) {
  static get $name() {
    return 'ProjectModelMixin';
  }
  //region Config
  static get defaultConfig() {
    return {
      /**
       * State tracking manager instance the project relies on
       * @member {Core.data.stm.StateTrackingManager} stm
       * @category Advanced
       */
      /**
       * Configuration options to provide to the STM manager
       *
       * @config {StateTrackingManagerConfig|Core.data.stm.StateTrackingManager}
       * @category Advanced
       */
      stm: {},
      timeRangeModelClass: TimeSpan,
      resourceTimeRangeModelClass: ResourceTimeRangeModel,
      /**
       * The constructor to create a time range store instance with. Should be a class subclassing the
       * {@link Core.data.Store}
       * @config {Core.data.Store|Object}
       * @typings {typeof Store|object}
       * @category Models & Stores
       */
      timeRangeStoreClass: Store,
      /**
       * The constructor to create a resource time range store instance with. Should be a class subclassing the
       * {@link Scheduler.data.ResourceTimeRangeStore}
       * @config {Scheduler.data.ResourceTimeRangeStore|Object}
       * @typings {typeof ResourceTimeRangeStore|object}
       * @category Models & Stores
       */
      resourceTimeRangeStoreClass: ResourceTimeRangeStore,
      /**
       * The initial data, to fill the {@link #property-timeRangeStore timeRangeStore} with.
       * Should be an array of {@link Scheduler.model.TimeSpan TimeSpan} or its configuration objects.
       *
       * @config {Scheduler.model.TimeSpan[]} [timeRangesData]
       * @category Legacy inline data
       */
      /**
       * The initial data, to fill the {@link #property-resourceTimeRangeStore resourceTimeRangeStore} with.
       * Should be an array of {@link Scheduler.model.ResourceTimeRangeModel ResourceTimeRangeModel} or it's
       * configuration objects.
       *
       * @config {Scheduler.model.ResourceTimeRangeModel[]} [resourceTimeRangesData]
       * @category Legacy inline data
       */
      eventStore: {},
      assignmentStore: {},
      dependencyStore: {},
      resourceStore: {},
      timeRangesData: null
    };
  }
  static configurable = {
    /**
     * Project data as a JSON string, used to populate its stores.
     *
     * ```javascript
     * const project = new ProjectModel({
     *     json : '{"eventsData":[...],"resourcesData":[...],...}'
     * }
     * ```
     *
     * @config {String}
     * @category Inline data
     */
    json: null,
    /**
     * The {@link Core.data.Store store} holding the time ranges information.
     *
     * See also {@link Scheduler.model.TimeSpan}
     *
     * @member {Core.data.Store} timeRangeStore
     * @category Models & Stores
     */
    /**
     * A {@link Core.data.Store} instance or a config object.
     * @config {Core.data.Store|StoreConfig}
     * @category Models & Stores
     */
    timeRangeStore: {
      value: {
        id: 'timeRanges',
        modelClass: TimeSpan
      },
      $config: 'nullify'
    },
    /**
     * The {@link Scheduler.data.ResourceTimeRangeStore store} holding the resource time ranges information.
     *
     * See also {@link Scheduler.model.ResourceTimeRangeModel}
     *
     * @member {Scheduler.data.ResourceTimeRangeStore} resourceTimeRangeStore
     * @category Models & Stores
     */
    /**
     * A {@link Scheduler.data.ResourceTimeRangeStore} instance or a config object.
     * @config {Scheduler.data.ResourceTimeRangeStore|ResourceTimeRangeStoreConfig}
     * @category Models & Stores
     */
    resourceTimeRangeStore: {
      value: {},
      $config: 'nullify'
    },
    // Documented in Scheduler/SchedulerPro versions of model/ProjectModel since types differ
    events: null,
    resourceTimeRanges: null
  };
  //endregion
  //region Properties
  /**
   * Get or set data of project stores. The returned data is identical to what
   * {@link #function-toJSON} returns:
   *
   * ```javascript
   *
   * const data = scheduler.project.inlineData;
   *
   * // data:
   * {
   *     eventsData             : [...],
   *     resourcesData          : [...],
   *     dependenciesData       : [...],
   *     assignmentsData        : [...],
   *     resourceTimeRangesData : [...],
   *     timeRangesData         : [...]
   * }
   *
   *
   * // Plug it back in later
   * scheduler.project.inlineData = data;
   * ```
   *
   * @property {Object}
   * @category Inline data
   */
  get inlineData() {
    return StringHelper.safeJsonParse(super.json);
  }
  set inlineData(inlineData) {
    this.json = inlineData;
  }
  //endregion
  //region Functions
  /**
   * Accepts a "data package" consisting of data for the projects stores, which is then loaded into the stores.
   *
   * The package can hold data for `EventStore`, `AssignmentStore`, `ResourceStore`, `DependencyStore`,
   * `TimeRangeStore` and `ResourceTimeRangeStore`. It uses the same format as when creating a project with inline
   * data:
   *
   * ```javascript
   * await project.loadInlineData({
   *     eventsData             : [...],
   *     resourcesData          : [...],
   *     assignmentsData        : [...],
   *     dependenciesData       : [...],
   *     resourceTimeRangesData : [...],
   *     timeRangesData         : [...]
   * });
   * ```
   *
   * After populating the stores it commits the project, starting its calculations. By awaiting `loadInlineData()` you
   * can be sure that project calculations are finished.
   *
   * @function loadInlineData
   * @param {Object} dataPackage A data package as described above
   * @fires load
   * @async
   * @category Inline data
   */
  /**
   * Project changes (CRUD operations to records in its stores) are automatically committed on a buffer to the
   * underlying graph based calculation engine. The engine performs it calculations async.
   *
   * By calling this function, the commit happens right away. And by awaiting it you are sure that project
   * calculations are finished and that references between records are up to date.
   *
   * The returned promise is resolved with an object. If that object has `rejectedWith` set, there has been a conflict and the calculation failed.
   *
   * ```javascript
   * // Move an event in time
   * eventStore.first.shift(1);
   *
   * // Trigger calculations directly and wait for them to finish
   * const result = await project.commitAsync();
   *
   * if (result.rejectedWith) {
   *     // there was a conflict during the scheduling
   * }
   * ```
   *
   * @async
   * @function commitAsync
   * @category Common
   */
  //endregion
  //region Init
  construct(config = {}) {
    super.construct(...arguments);
    // These stores are not handled by engine, but still held on project
    if (config.timeRangesData) {
      this.timeRangeStore.data = config.timeRangesData;
    }
    if (config.resourceTimeRangesData) {
      this.resourceTimeRangeStore.data = config.resourceTimeRangesData;
    }
  }
  afterConstruct() {
    super.afterConstruct();
    const me = this;
    !me.timeRangeStore.stm && me.stm.addStore(me.timeRangeStore);
    !me.resourceTimeRangeStore.stm && me.stm.addStore(me.resourceTimeRangeStore);
  }
  //endregion
  //region Attaching stores
  // Attach to a store, relaying its change events
  attachStore(store) {
    if (store) {
      store.ion({
        name: store.$$name,
        change: 'relayStoreChange',
        thisObj: this
      });
    }
    super.attachStore(store);
  }
  // Detach a store, stop relaying its change events
  detachStore(store) {
    if (store) {
      this.detachListeners(store.$$name);
      super.detachStore(store);
    }
  }
  relayStoreChange(event) {
    super.relayStoreChange(event);
    /**
     * Fired when data in any of the projects stores changes.
     *
     * Basically a relayed version of each stores own change event, decorated with which store it originates from.
     * See the {@link Core.data.Store#event-change store change event} documentation for more information.
     *
     * @event change
     * @param {Scheduler.model.ProjectModel} source This project
     * @param {Core.data.Store} store Affected store
     * @param {'remove'|'removeAll'|'add'|'updatemultiple'|'clearchanges'|'filter'|'update'|'dataset'|'replace'} action
     * Name of action which triggered the change. May be one of:
     * * `'remove'`
     * * `'removeAll'`
     * * `'add'`
     * * `'updatemultiple'`
     * * `'clearchanges'`
     * * `'filter'`
     * * `'update'`
     * * `'dataset'`
     * * `'replace'`
     * @param {Core.data.Model} record Changed record, for actions that affects exactly one record (`'update'`)
     * @param {Core.data.Model[]} records Changed records, passed for all actions except `'removeAll'`
     * @param {Object} changes Passed for the `'update'` action, info on which record fields changed
     */
    return this.trigger('change', {
      store: event.source,
      ...event,
      source: this
    });
  }
  updateTimeRangeStore(store, oldStore) {
    this.detachStore(oldStore);
    this.attachStore(store);
  }
  setTimeRangeStore(store) {
    this.timeRangeStore = store;
  }
  changeTimeRangeStore(store) {
    // If it's not being nullified, upgrade a config object to be a full store
    if (store && !store.isStore) {
      store = this.timeRangeStoreClass.new({
        modelClass: this.timeRangeModelClass
      }, store);
    }
    return store;
  }
  updateResourceTimeRangeStore(store, oldStore) {
    this.detachStore(oldStore);
    this.attachStore(store);
  }
  changeResourceTimeRangeStore(store) {
    // If it's not being nullified, upgrade a config object to be a full store
    if (store && !store.isStore) {
      store = this.resourceTimeRangeStoreClass.new({
        modelClass: this.resourceTimeRangeModelClass
      }, store);
    }
    return store;
  }
  setResourceTimeRangeStore(store) {
    this.resourceTimeRangeStore = store;
  }
  //endregion
  //region Inline data
  get events() {
    return this.eventStore.allRecords;
  }
  updateEvents(events) {
    this.eventStore.data = events;
  }
  get resourceTimeRanges() {
    return this.resourceTimeRangeStore.allRecords;
  }
  updateResourceTimeRanges(resourceTimeRanges) {
    this.resourceTimeRangeStore.data = resourceTimeRanges;
  }
  async loadInlineData(data) {
    // Flag reset in super
    this.isLoadingInlineData = true;
    // Stores not handled by engine
    if (data.resourceTimeRangesData) {
      this.resourceTimeRangeStore.data = data.resourceTimeRangesData;
    }
    if (data.timeRangesData) {
      this.timeRangeStore.data = data.timeRangesData;
    }
    return super.loadInlineData(data);
  }
  //endregion
  //region JSON
  /**
   * Returns the data from the records of the projects stores, in a format that can be consumed by `loadInlineData()`.
   *
   * Used by JSON.stringify to correctly convert this record to json.
   *
   *
   * ```javascript
   * const project = new ProjectModel({
   *     eventsData             : [...],
   *     resourcesData          : [...],
   *     assignmentsData        : [...],
   *     dependenciesData       : [...],
   *     resourceTimeRangesData : [...],
   *     timeRangesData         : [...]
   * });
   *
   * const json = project.toJSON();
   *
   * // json:
   * {
   *     eventsData             : [...],
   *     resourcesData          : [...],
   *     dependenciesData       : [...],
   *     assignmentsData        : [...],
   *     resourceTimeRangesData : [...],
   *     timeRangesData         : [...]
   * }
   * ```
   *
   * Output can be consumed by `loadInlineData()`:
   *
   * ```javascript
   * const json = project.toJSON();
   *
   * // Plug it back in later
   * project.loadInlineData(json);
   * ```
   *
   * @returns {Object}
   * @category Inline data
   */
  toJSON() {
    const me = this,
      result = {
        eventsData: me.eventStore.toJSON(),
        resourcesData: me.resourceStore.toJSON(),
        dependenciesData: me.dependencyStore.toJSON(),
        timeRangesData: me.timeRangeStore.toJSON(),
        resourceTimeRangesData: me.resourceTimeRangeStore.toJSON()
      };
    if (!me.eventStore.usesSingleAssignment) {
      result.assignmentsData = me.assignmentStore.toJSON();
    }
    return result;
  }
  /**
   * Get or set project data (records from its stores) as a JSON string.
   *
   * Get a JSON string:
   *
   * ```javascript
   * const project = new ProjectModel({
   *     eventsData             : [...],
   *     resourcesData          : [...],
   *     assignmentsData        : [...],
   *     dependenciesData       : [...],
   *     resourceTimeRangesData : [...],
   *     timeRangesData         : [...]
   * });
   *
   * const jsonString = project.json;
   *
   * // jsonString:
   * '{"eventsData":[...],"resourcesData":[...],...}'
   * ```
   *
   * Set a JSON string (to populate the project stores):
   *
   * ```javascript
   * project.json = '{"eventsData":[...],"resourcesData":[...],...}'
   * ```
   *
   * @property {String}
   * @category Inline data
   */
  get json() {
    return super.json;
  }
  changeJson(json) {
    if (typeof json === 'string') {
      json = StringHelper.safeJsonParse(json);
    }
    return json;
  }
  updateJson(json) {
    json && this.loadInlineData(json);
  }
  //endregion
  afterChange(toSet, wasSet) {
    super.afterChange(...arguments);
    if (wasSet.calendar) {
      this.trigger('calendarChange');
    }
  }
  doDestroy() {
    this.timeRangeStore.destroy();
    this.resourceTimeRangeStore.destroy();
    super.doDestroy();
  }
});

/**
 * @module Scheduler/model/mixin/ProjectCurrentConfig
 */
/**
 * Mixin that makes sure current config for a project includes store data and is cleaned up properly.
 *
 * @mixin
 * @private
 */
var ProjectCurrentConfig = (Target => class ProjectCurrentConfig extends Target {
  // This function is not meant to be called by any code other than Base#getCurrentConfig().
  // It extracts the current configs/fields for the project, with special handling for inline data
  getCurrentConfig(options) {
    const me = this,
      result = super.getCurrentConfig(options);
    if (result) {
      for (const storeName of ['eventStore', 'resourceStore', 'assignmentStore', 'dependencyStore', 'timeRangeStore', 'resourceTimeRangeStore']) {
        const store = me[storeName];
        if (store) {
          if (store.count) {
            result[store.id + 'Data'] = store.getInlineData(options);
          }
          // Get stores current state, in case it has filters etc. added at runtime
          const storeState = store.getCurrentConfig(options);
          if (storeState && Object.keys(storeState).length > 0) {
            result[storeName] = Object.assign(result[storeName] || {}, storeState);
          }
          // Remove empty store configs
          else if (result[storeName] && Object.keys(result[storeName]).length === 0) {
            delete result[storeName];
          }
        }
      }
      if (result.timeRangeStore) {
        // Exclude default time range modelClass (it is a plain store), spam
        if (me.timeRangeStore.originalModelClass === me.timeRangeModelClass || me.timeRangeStore.originalModelClass.$name === 'TimeSpan') {
          delete result.timeRangeStore.modelClass;
        }
        // Same for default storeId
        if (result.timeRangeStore.storeId === 'timeRanges') {
          delete result.timeRangeStore.storeId;
        }
        // Since timeRangeStore is a plain store it is always configured with id, spam
        if (Object.keys(result.timeRangeStore).length === 1) {
          delete result.timeRangeStore;
        }
      }
      // Gantt specifics
      if (me.taskStore.isTaskStore) {
        delete result.eventModelClass;
        delete result.eventStoreClass;
        delete result.children;
      }
      return result;
    }
  }
});

/**
 * @module Scheduler/data/util/ModelPersistencyManager
 */
/**
 * This class manages model persistency, it listens to model stores' beforesync event and removes all non persistable
 * records from sync operation. The logic has meaning only for CRUD-less sync operations.
 *
 * @private
 */
class ModelPersistencyManager extends Base {
  // region Event attachers
  set eventStore(newEventStore) {
    const me = this;
    me.eventStoreDetacher && me.eventStoreDetacher();
    me._eventStore = newEventStore;
    if (newEventStore && newEventStore.autoCommit) {
      me.eventStoreDetacher = newEventStore.ion({
        beforecommit: me.onEventStoreBeforeSync,
        thisObj: me,
        detachable: true,
        // Just in case
        prio: 100
      });
    }
  }
  get eventStore() {
    return this._eventStore;
  }
  set resourceStore(newResourceStore) {
    const me = this;
    me.resourceStoreDetacher && me.resourceStoreDetacher();
    me._resourceStore = newResourceStore;
    if (newResourceStore && newResourceStore.autoCommit) {
      me.resourceStoreDetacher = newResourceStore.ion({
        beforecommit: me.onResourceStoreBeforeSync,
        thisObj: me,
        detachable: true,
        // Just in case
        prio: 100
      });
    }
  }
  get resourceStore() {
    return this._resourceStore;
  }
  set assignmentStore(newAssignmentStore) {
    const me = this;
    me.assignmentStoreDetacher && me.assignmentStoreDetacher();
    me._assignmentStore = newAssignmentStore;
    if (newAssignmentStore && newAssignmentStore.autoSync) {
      me.assignmentStoreDetacher = newAssignmentStore.ion({
        beforecommit: me.onAssignmentStoreBeforeSync,
        thisObj: me,
        detachable: true,
        // Just in case
        prio: 100
      });
    }
  }
  get assignmentStore() {
    return this._assignmentStore;
  }
  set dependencyStore(newDependencyStore) {
    const me = this;
    me.dependencyStoreDetacher && me.dependencyStoreDetacher();
    me._dependencyStore = newDependencyStore;
    if (newDependencyStore && newDependencyStore.autoSync) {
      me.dependencyStoreDetacher = newDependencyStore.ion({
        beforecommit: me.onDependencyStoreBeforeSync,
        thisObj: me,
        detachable: true,
        // Just in case
        prio: 100
      });
    }
  }
  get dependencyStore() {
    return this._dependencyStore;
  }
  // endregion
  // region Event handlers
  onEventStoreBeforeSync({
    changes
  }) {
    const me = this;
    me.removeNonPersistableRecordsToCreate(changes);
    return me.shallContinueSync(changes);
  }
  onResourceStoreBeforeSync({
    changes
  }) {
    const me = this;
    me.removeNonPersistableRecordsToCreate(changes);
    return me.shallContinueSync(changes);
  }
  onAssignmentStoreBeforeSync({
    changes
  }) {
    const me = this;
    me.removeNonPersistableRecordsToCreate(changes);
    return me.shallContinueSync(changes);
  }
  onDependencyStoreBeforeSync({
    changes
  }) {
    const me = this;
    me.removeNonPersistableRecordsToCreate(changes);
    return me.shallContinueSync(changes);
  }
  // endregion
  // region Management rules
  removeNonPersistableRecordsToCreate(changes) {
    const recordsToCreate = changes.added || [];
    let r, i;
    // We remove from the array we iterate thus we iterate from end to start
    for (i = recordsToCreate.length - 1; i >= 0; --i) {
      r = recordsToCreate[i];
      if (!r.isPersistable) {
        recordsToCreate.splice(recordsToCreate.indexOf(r), 1);
      }
    }
    // Prevent empty create request
    if (recordsToCreate.length === 0) {
      changes.added.length = 0;
    }
  }
  shallContinueSync(options) {
    return Boolean(options.added && options.added.length > 0 || options.modified && options.modified.length > 0 || options.removed && options.removed.length > 0);
  }
  // endregion
}

ModelPersistencyManager._$name = 'ModelPersistencyManager';

const EngineMixin = SchedulerCoreProjectMixin;
/**
 * @module Scheduler/model/ProjectModel
 */
/**
 * This class represents a global project of your Scheduler - a central place for all data.
 *
 * It holds and links the stores usually used by Scheduler:
 *
 * - {@link Scheduler.data.EventStore}
 * - {@link Scheduler.data.ResourceStore}
 * - {@link Scheduler.data.AssignmentStore}
 * - {@link Scheduler.data.DependencyStore}
 * - {@link #config-timeRangeStore TimeRangeStore}
 * - {@link Scheduler.data.ResourceTimeRangeStore}
 *
 * The project uses a calculation engine to normalize dates and durations. It is also responsible for
 * handling references between models, for example to link an event via an assignment to a resource. These operations
 * are asynchronous, a fact that is hidden when working in the Scheduler UI but which you must know about when performing
 * more advanced operations on the data level.
 *
 * When there is a change to data that requires something else to be recalculated, the project schedules a calculation (a
 * commit) which happens moments later. It is also possible to trigger these calculations directly. This snippet illustrate
 * the process:
 *
 1. Something changes which requires the project to recalculate, for example adding a new task:
 *
 * ```javascript
 * const [event] = project.eventStore.add({ startDate, endDate });
 * ```
 *
 * 2. A recalculation is scheduled, thus:
 *
 * ```javascript
 * event.duration; // <- Not yet calculated
 * ```
 *
 * 3. Calculate now instead of waiting for the scheduled calculation
 *
 * ```javascript
 * await project.commitAsync();
 *
 * event.duration; // <- Now available
 * ```
 *
 * ## Using inline data
 *
 * The project provides settable property {@link Scheduler.crud.AbstractCrudManager#property-inlineData} that can
 * be used to get data from all its stores at once and to set this data as well. Populating the stores this way can
 * be useful if you cannot or you do not want to use CrudManager for server requests but you pull the data by other
 * means and have it ready outside of ProjectModel. Also, the data from all stores is available in a single
 * assignment statement.
 *
 * ### Getting data
 * ```javascript
 * const data = scheduler.project.inlineData;
 *
 * // use the data in your application
 * ```
 *
 * ### Setting data
 * ```javascript
 * const data = // your function to pull server data
 *
 * scheduler.project.inlineData = data;
 * ```
 *
 * ## Monitoring data changes
 *
 * While it is possible to listen for data changes on the projects individual stores, it is sometimes more convenient
 * to have a centralized place to handle all data changes. By listening for the {@link #event-change change event} your
 * code gets notified when data in any of the stores changes. Useful for example to keep an external data model up to
 * date:
 *
 * ```javascript
 * const scheduler = new Scheduler({
 *     project: {
 *         listeners : {
 *             change({ store, action, records }) {
 *                 const { $name } = store.constructor;
 *
 *                 if (action === 'add') {
 *                     externalDataModel.add($name, records);
 *                 }
 *
 *                 if (action === 'remove') {
 *                     externalDataModel.remove($name, records);
 *                 }
 *             }
 *         }
 *     }
 * });
 * ```
 *
 * ## Built in StateTrackingManager
 *
 * The project also has a built in {@link Core.data.stm.StateTrackingManager StateTrackingManager} (STM for short), that
 * handles undo/redo for the project stores (additional stores can also be added). You can enable it to track all
 * project store changes:
 *
 * ```javascript
 * // Turn on auto recording when you create your Scheduler:
 * const scheduler = new Scheduler({
 *    project : {
 *        stm : {
 *            autoRecord : true
 *        }
 *    }
 * });
 *
 * // Undo a transaction
 * project.stm.undo();
 *
 * // Redo
 * project.stm.redo();
 * ```
 *
 * Check out the `undoredo` demo to see it in action.
 *
 * @extends Core/data/Model
 * @mixes Scheduler/model/mixin/ProjectModelMixin
 * @uninherit Core/data/mixin/TreeNode
 */
class ProjectModel extends ProjectCurrentConfig(ProjectModelMixin(EngineMixin)) {
  static get $name() {
    return 'ProjectModel';
  }
  //region Inline data configs & properties
  /**
   * @hidefields id, readOnly, children, parentId, parentIndex
   */
  /**
   * Get/set {@link #property-eventStore} data.
   *
   * Always returns an array of {@link Scheduler.model.EventModel EventModels} but also accepts an array of
   * its configuration objects as input.
   *
   * @member {Scheduler.model.EventModel[]} events
   * @accepts {Scheduler.model.EventModel[]|EventModelConfig[]}
   * @category Inline data
   */
  /**
   * Data use to fill the {@link #property-eventStore}. Should be an array of
   * {@link Scheduler.model.EventModel EventModels} or its configuration objects.
   *
   * @config {Scheduler.model.EventModel[]|EventModelConfig[]} events
   * @category Inline data
   */
  /**
   * Get/set {@link #property-resourceStore} data.
   *
   * Always returns an array of {@link Scheduler.model.ResourceModel ResourceModels} but also accepts an array
   * of its configuration objects as input.
   *
   * @member {Scheduler.model.ResourceModel[]} resources
   * @accepts {Scheduler.model.ResourceModel[]|ResourceModelConfig[]}
   * @category Inline data
   */
  /**
   * Data use to fill the {@link #property-resourceStore}. Should be an array of
   * {@link Scheduler.model.ResourceModel ResourceModels} or its configuration objects.
   *
   * @config {Scheduler.model.ResourceModel[]|ResourceModelConfig[]} resources
   * @category Inline data
   */
  /**
   * Get/set {@link #property-assignmentStore} data.
   *
   * Always returns an array of {@link Scheduler.model.AssignmentModel AssignmentModels} but also accepts an
   * array of its configuration objects as input.
   *
   * @member {Scheduler.model.AssignmentModel[]} assignments
   * @accepts {Scheduler.model.AssignmentModel[]|AssignmentModelConfig[]}
   * @category Inline data
   */
  /**
   * Data use to fill the {@link #property-assignmentStore}. Should be an array of
   * {@link Scheduler.model.AssignmentModel AssignmentModels} or its configuration objects.
   *
   * @config {Scheduler.model.AssignmentModel[]|AssignmentModelConfig[]} assignments
   * @category Inline data
   */
  /**
   * Get/set {@link #property-dependencyStore} data.
   *
   * Always returns an array of {@link Scheduler.model.DependencyModel DependencyModels} but also accepts an
   * array of its configuration objects as input.
   *
   * @member {Scheduler.model.DependencyModel[]} dependencies
   * @accepts {Scheduler.model.DependencyModel[]|DependencyModelConfig[]}
   * @category Inline data
   */
  /**
   * Data use to fill the {@link #property-dependencyStore}. Should be an array of
   * {@link Scheduler.model.DependencyModel DependencyModels} or its configuration objects.
   *
   * @config {Scheduler.model.DependencyModel[]|DependencyModelConfig[]} dependencies
   * @category Inline data
   */
  /**
   * Get/set {@link #property-timeRangeStore} data.
   *
   * Always returns an array of {@link Scheduler.model.TimeSpan TimeSpans} but also accepts an
   * array of its configuration objects as input.
   *
   * @member {Scheduler.model.TimeSpan[]} timeRanges
   * @accepts {Scheduler.model.TimeSpan[]|TimeSpanConfig[]}
   * @category Inline data
   */
  /**
   * Data use to fill the {@link #property-timeRangeStore}. Should be an array of
   * {@link Scheduler.model.TimeSpan TimeSpans} or its configuration objects.
   *
   * @config {Scheduler.model.TimeSpan[]|TimeSpanConfig[]} timeRanges
   * @category Inline data
   */
  /**
   * Get/set {@link #property-resourceTimeRangeStore} data.
   *
   * Always returns an array of {@link Scheduler.model.ResourceTimeRangeModel ResourceTimeRangeModels} but
   * also accepts an array of its configuration objects as input.
   *
   * @member {Scheduler.model.ResourceTimeRangeModel[]} resourceTimeRanges
   * @accepts {Scheduler.model.ResourceTimeRangeModel[]|ResourceTimeRangeModelConfig[]}
   * @category Inline data
   */
  /**
   * Data use to fill the {@link #property-resourceTimeRangeStore}. Should be an array
   * of {@link Scheduler.model.ResourceTimeRangeModel ResourceTimeRangeModels} or its configuration objects.
   *
   * @config {Scheduler.model.ResourceTimeRangeModel[]|ResourceTimeRangeModelConfig[]} resourceTimeRanges
   * @category Inline data
   */
  //endregion
  //region Legacy inline data configs & properties
  /**
   * The initial data, to fill the {@link #property-eventStore} with.
   * Should be an array of {@link Scheduler.model.EventModel EventModels} or its configuration objects.
   *
   * @config {Scheduler.model.EventModel[]|EventModelConfig[]} eventsData
   * @category Legacy inline data
   */
  /**
   * The initial data, to fill the {@link #property-dependencyStore} with.
   * Should be an array of {@link Scheduler.model.DependencyModel DependencyModels} or its configuration
   * objects.
   *
   * @config {Scheduler.model.DependencyModel[]|DependencyModelConfig[]} [dependenciesData]
   * @category Legacy inline data
   */
  /**
   * The initial data, to fill the {@link #property-resourceStore} with.
   * Should be an array of {@link Scheduler.model.ResourceModel ResourceModels} or its configuration objects.
   *
   * @config {Scheduler.model.ResourceModel[]|ResourceModelConfig[]} [resourcesData]
   * @category Legacy inline data
   */
  /**
   * The initial data, to fill the {@link #property-assignmentStore} with.
   * Should be an array of {@link Scheduler.model.AssignmentModel AssignmentModels} or its configuration
   * objects.
   *
   * @config {Scheduler.model.AssignmentModel[]|AssignmentModelConfig[]} [assignmentsData]
   * @category Legacy inline data
   */
  //endregion
  //region Store configs & properties
  /**
   * The {@link Scheduler.data.EventStore store} holding the events information.
   *
   * See also {@link Scheduler.model.EventModel}
   *
   * @member {Scheduler.data.EventStore} eventStore
   * @category Models & Stores
   */
  /**
   * An {@link Scheduler.data.EventStore} instance or a config object.
   * @config {Scheduler.data.EventStore|EventStoreConfig} eventStore
   * @category Models & Stores
   */
  /**
   * The {@link Scheduler.data.DependencyStore store} holding the dependencies information.
   *
   * See also {@link Scheduler.model.DependencyModel}
   *
   * @member {Scheduler.data.DependencyStore} dependencyStore
   * @category Models & Stores
   */
  /**
   * A {@link Scheduler.data.DependencyStore} instance or a config object.
   * @config {Scheduler.data.DependencyStore|DependencyStoreConfig} dependencyStore
   * @category Models & Stores
   */
  /**
   * The {@link Scheduler.data.ResourceStore store} holding the resources that can be assigned to the events in the event store.
   *
   * See also {@link Scheduler.model.ResourceModel}
   *
   * @member {Scheduler.data.ResourceStore} resourceStore
   * @category Models & Stores
   */
  /**
   * A {@link Scheduler.data.ResourceStore} instance or a config object.
   * @config {Scheduler.data.ResourceStore|ResourceStoreConfig} resourceStore
   * @category Models & Stores
   */
  /**
   * The {@link Scheduler.data.AssignmentStore store} holding the assignments information.
   *
   * See also {@link Scheduler.model.AssignmentModel}
   *
   * @member {Scheduler.data.AssignmentStore} assignmentStore
   * @category Models & Stores
   */
  /**
   * An {@link Scheduler.data.AssignmentStore} instance or a config object.
   * @config {Scheduler.data.AssignmentStore|AssignmentStoreConfig} assignmentStore
   * @category Models & Stores
   */
  //endregion
  //region Configs
  static get defaultConfig() {
    return {
      /**
       * The constructor of the event model class, to be used in the project. Will be set as the
       * {@link Core.data.Store#config-modelClass modelClass} property of the {@link #property-eventStore}
       *
       * @config {Scheduler.model.EventModel}
       * @typings {typeof EventModel}
       * @category Models & Stores
       */
      eventModelClass: EventModel,
      /**
       * The constructor of the dependency model class, to be used in the project. Will be set as the
       * {@link Core.data.Store#config-modelClass modelClass} property of the {@link #property-dependencyStore}
       *
       * @config {Scheduler.model.DependencyModel}
       * @typings {typeof DependencyModel}
       * @category Models & Stores
       */
      dependencyModelClass: DependencyModel,
      /**
       * The constructor of the resource model class, to be used in the project. Will be set as the
       * {@link Core.data.Store#config-modelClass modelClass} property of the {@link #property-resourceStore}
       *
       * @config {Scheduler.model.ResourceModel}
       * @typings {typeof ResourceModel}
       * @category Models & Stores
       */
      resourceModelClass: ResourceModel,
      /**
       * The constructor of the assignment model class, to be used in the project. Will be set as the
       * {@link Core.data.Store#config-modelClass modelClass} property of the {@link #property-assignmentStore}
       *
       * @config {Scheduler.model.AssignmentModel}
       * @typings {typeof AssignmentModel}
       * @category Models & Stores
       */
      assignmentModelClass: AssignmentModel,
      /**
       * The constructor to create an event store instance with. Should be a class, subclassing the
       * {@link Scheduler.data.EventStore}
       * @config {Scheduler.data.EventStore|Object}
       * @typings {typeof EventStore|object}
       * @category Models & Stores
       */
      eventStoreClass: EventStore,
      /**
       * The constructor to create a dependency store instance with. Should be a class, subclassing the
       * {@link Scheduler.data.DependencyStore}
       * @config {Scheduler.data.DependencyStore|Object}
       * @typings {typeof DependencyStore|object}
       * @category Models & Stores
       */
      dependencyStoreClass: DependencyStore,
      /**
       * The constructor to create a resource store instance with. Should be a class, subclassing the
       * {@link Scheduler.data.ResourceStore}
       * @config {Scheduler.data.ResourceStore|Object}
       * @typings {typeof ResourceStore|object}
       * @category Models & Stores
       */
      resourceStoreClass: ResourceStore,
      /**
       * The constructor to create an assignment store instance with. Should be a class, subclassing the
       * {@link Scheduler.data.AssignmentStore}
       * @config {Scheduler.data.AssignmentStore|Object}
       * @typings {typeof AssignmentStore|object}
       * @category Models & Stores
       */
      assignmentStoreClass: AssignmentStore
    };
  }
  //endregion
  //region Events
  /**
   * Fired when the engine has finished its calculations and the results has been written back to the records.
   *
   * ```javascript
   * scheduler.project.on({
   *     dataReady() {
   *        console.log('Calculations finished');
   *     }
   * });
   *
   * scheduler.eventStore.first.duration = 10;
   *
   * // At some point a bit later it will log 'Calculations finished'
   * ```
   *
   * @event dataReady
   * @param {Scheduler.model.ProjectModel} source The project
   */
  //endregion
  /**
   * Silences propagations caused by the project loading.
   *
   * Applying the loaded data to the project occurs in two basic stages:
   *
   * 1. Data gets into the engine graph which triggers changes propagation
   * 2. The changes caused by the propagation get written to related stores
   *
   * Setting this flag to `true` makes the component perform step 2 silently without triggering events causing reactions on those changes
   * (like sending changes back to the server if `autoSync` is enabled) and keeping stores in unmodified state.
   *
   * This is safe if the loaded data is consistent so propagation doesn't really do any adjustments.
   * By default the system treats the data as consistent so this option is `true`.
   *
   * ```js
   * new Scheduler({
   *     project : {
   *         // We want scheduling engine to recalculate the data properly
   *         // so then we could save it back to the server
   *         silenceInitialCommit : false
   *     }
   *     ...
   * })
   * ```
   *
   * @config {Boolean} silenceInitialCommit
   * @default true
   * @category Advanced
   */
  construct(...args) {
    super.construct(...args);
    if (VersionHelper.isTestEnv) {
      globalThis.bryntum.testProject = this;
    }
    // Moved here from EventStore, since project is now the container instead of it
    this.modelPersistencyManager = this.createModelPersistencyManager();
  }
  /**
   * Creates and returns model persistency manager
   *
   * @returns {Scheduler.data.util.ModelPersistencyManager}
   * @internal
   */
  createModelPersistencyManager() {
    return new ModelPersistencyManager({
      eventStore: this,
      resourceStore: this.resourceStore,
      assignmentStore: this.assignmentStore,
      dependencyStore: this.dependencyStore
    });
  }
  doDestroy() {
    this.modelPersistencyManager.destroy();
    super.doDestroy();
  }
  // To comply with TaskBoards expectations
  get taskStore() {
    return this.eventStore;
  }
}
ProjectModel.applyConfigs = true;
ProjectModel.initClass();
ProjectModel._$name = 'ProjectModel';

export { AbstractAssignmentStoreMixin, AbstractCalendarManagerStoreMixin, AbstractCalendarMixin, AbstractCrudManagerMixin, AbstractCrudManagerValidation, AbstractDependencyStoreMixin, AbstractEventStoreMixin, AbstractHasAssignmentsMixin, AbstractPartOfProjectGenericMixin, AbstractPartOfProjectModelMixin, AbstractPartOfProjectStoreMixin, AbstractProjectMixin, AbstractRecurrenceIterator, AbstractResourceStoreMixin, AjaxTransport, AssignmentModel, AssignmentModelMixin, AssignmentStore, AssignmentStoreMixin, CI, CIFromSetOrArrayOrValue, CalendarCache, CalendarCacheInterval, CalendarCacheSingle, CalendarIntervalMixin, CalendarIntervalStore, CalendarIteratorResult, ConstraintIntervalSide, ConstraintType, CoreAssignmentMixin, CoreAssignmentStoreMixin, CoreCalendarManagerStoreMixin, CoreCalendarMixin, CoreDependencyMixin, CoreDependencyStoreMixin, CoreEventMixin, CoreEventStoreMixin, CoreHasAssignmentsMixin, CoreHasDependenciesMixin, CorePartOfProjectGenericMixin, CorePartOfProjectModelMixin, CorePartOfProjectStoreMixin, CoreResourceMixin, CoreResourceStoreMixin, CrudManagerView, DailyRecurrenceIterator, DayIndexMixin, DelayableWrapper, DependenciesCalendar, DependencyBaseModel, DependencyModel, DependencyStore, DependencyStoreMixin, DependencyType, DependencyValidationResult, Direction, EdgeInclusion, EventDayIndex, EventModel, EventModelMixin, EventStore, EventStoreMixin, GetEventsMixin, IndexPosition, IntervalCache, JsonEncoder, MAX_DATE, MI, MIN_DATE, Mixin, MixinAny, MonthlyRecurrenceIterator, PartOfProject, ProjectCrudManager, ProjectCurrentConfig, ProjectModel, ProjectModelCommon, ProjectModelMixin, ProjectModelTimeZoneMixin, ProjectType, RecurrenceDayRuleEncoder, RecurrenceModel, RecurringEventsMixin, RecurringTimeSpan, RecurringTimeSpansMixin, ResourceModel, ResourceModelMixin, ResourceStore, ResourceStoreMixin, ResourceTimeRangeModel, ResourceTimeRangeStore, SchedulerCoreEvent, SchedulerCoreProjectMixin, SchedulingMode, SharedEventStoreMixin, SortedMap, TimeSpan, TimeUnit, TimeZonedDatesMixin, UnspecifiedTimeIntervalModel, WeeklyRecurrenceIterator, YearlyRecurrenceIterator, binarySearch, concat, concatIterable, delay, format, isDateFinite, isInstanceOf, isNotNumber, later, map, stripDuplicates, uniqueOnly };
//# sourceMappingURL=ProjectModel.js.map
