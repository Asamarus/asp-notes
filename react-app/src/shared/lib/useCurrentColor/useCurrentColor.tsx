import useAppSelector from '@/shared/lib/useAppSelector'
import { useComputedColorScheme } from '@mantine/core'

function useCurrentColor() {
  const colorScheme = useComputedColorScheme('light')
  const sectionColor = useAppSelector(
    (state) => state.sections.current?.color ?? 'var(--mantine-primary-color-6)',
  )
  const color = colorScheme === 'dark' ? '#3b3b3b' : sectionColor

  return color
}

export default useCurrentColor
