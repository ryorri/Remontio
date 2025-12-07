<template>
  <div>
    <!-- Calculation Type 3 -->
    paint
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
  submit: [data: { calculationName: string; totalEfficiency: number }]
}>()
</script>

<style scoped></style>
