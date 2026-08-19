import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

export function formatCount(count: number): string {
  if (count >= 1_000_000) {
    return `${(count / 1_000_000).toFixed(1)}M`;
  }
  if (count >= 1_000) {
    return `${(count / 1_000).toFixed(1)}K`;
  }
  return String(count);
}

export function formatRelativeTime(isoDate: string, locale: string): string {
  const date = new Date(isoDate);
  const diffMs = Date.now() - date.getTime();
  const diffSec = Math.max(0, Math.floor(diffMs / 1000));

  const units: [Intl.RelativeTimeFormatUnit, number][] = [
    ["year", 31536000],
    ["month", 2592000],
    ["week", 604800],
    ["day", 86400],
    ["hour", 3600],
    ["minute", 60],
  ];

  const rtf = new Intl.RelativeTimeFormat(locale, {numeric: "auto"});

  for (const [unit, secondsInUnit] of units) {
    const value = Math.floor(diffSec / secondsInUnit);
    if (value >= 1) {
      return rtf.format(-value, unit);
    }
  }
  return rtf.format(0, "second");
}
