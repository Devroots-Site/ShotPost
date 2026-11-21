import { h } from 'vue'
import type { Theme } from 'vitepress'
import DefaultTheme from 'vitepress/theme'
import LastUpdatedBadge from './LastUpdatedBadge.vue'
import './custom.css'

export default {
  extends: DefaultTheme,
  // Badge mit dem Aenderungsdatum ueber jeder Doku-Seite.
  Layout: () => h(DefaultTheme.Layout, null, { 'doc-before': () => h(LastUpdatedBadge) }),
} satisfies Theme
