<template>
  <div>
    <!-- Floor calculation (Type 1) -->
    Floor
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue'

defineProps<{
  roomId: string
  submitting: boolean
}>()

const emit = defineEmits<{
  back: []
  submit: [data: { calculationName: string; value: number }]
}>()
</script>

<style scoped></style>
