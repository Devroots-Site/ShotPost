<script setup lang="ts">
import { computed } from 'vue'
import { useData } from 'vitepress'

const { page } = useData()

// Feste Zeitzone, damit Build und Browser dasselbe Datum rendern.
const format = new Intl.DateTimeFormat('de-DE', { dateStyle: 'medium', timeZone: 'Europe/Berlin' })
const date = computed(() =>
  page.value.lastUpdated ? format.format(new Date(page.value.lastUpdated)) : null,
)
</script>

<template>
  <div class="last-updated-badge">
    <Badge v-if="date" type="tip" :text="`Zuletzt aktualisiert: ${date}`" />
    <Badge v-else type="warning" text="Noch nicht committet" />
  </div>
</template>
