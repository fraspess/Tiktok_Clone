import { readFileSync } from 'node:fs'

const readLocale = (language) => JSON.parse(readFileSync(
    new URL(`../src/locales/${language}/${language}.json`, import.meta.url), 'utf8',
))
const kind = (value) => value === null ? 'null' : Array.isArray(value) ? 'array' : typeof value
const placeholders = (value) => [...value.matchAll(/{{\s*([^{}]+?)\s*}}/g)]
    .map((match) => match[1].trim()).sort()
const errors = []
let strings = 0

function compare(en, uk, path = '') {
    if (kind(en) !== kind(uk)) {
        errors.push(`${path}: different value types (${kind(en)} / ${kind(uk)})`)
        return
    }
    if (en !== null && typeof en === 'object') {
        for (const key of new Set([...Object.keys(en), ...Object.keys(uk)])) {
            const childPath = path ? `${path}.${key}` : key
            if (!Object.hasOwn(en, key)) errors.push(`${childPath}: missing in en`)
            else if (!Object.hasOwn(uk, key)) errors.push(`${childPath}: missing in uk`)
            else compare(en[key], uk[key], childPath)
        }
    } else if (typeof en === 'string') {
        strings++
        if (JSON.stringify(placeholders(en)) !== JSON.stringify(placeholders(uk))) {
            errors.push(`${path}: different interpolation placeholders`)
        }
    }
}

compare(readLocale('en'), readLocale('uk'))
if (errors.length) {
    console.error(errors.join('\n'))
    process.exitCode = 1
} else {
    console.log(`EN/UK locales match: ${strings} strings; nested keys, types, and placeholders checked.`)
}
