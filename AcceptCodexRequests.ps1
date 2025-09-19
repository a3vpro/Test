param(
  [string]$Remote = "origin",
  [string]$Main   = "main",
  # Mind the exact casing of your branch prefix:
  [string]$Prefix = "codex/",
  # Optional: use fast-forward only instead of creating merge commits
  [switch]$FastForwardOnly
)

# --- Ensure readable UTF-8 output (avoids mojibake in some consoles) ---
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}
try { chcp 65001 > $null 2>&1 } catch {}

Write-Host "[INFO] Verifying Git repository..."
git rev-parse --is-inside-work-tree *> $null
if ($LASTEXITCODE -ne 0) {
  Write-Error "[ERROR] This does not look like a Git repository."
  exit 1
}

$startBranch = (git rev-parse --abbrev-ref HEAD).Trim()

Write-Host "[INFO] Fetching and pruning remotes..."
git fetch --all --prune
if ($LASTEXITCODE -ne 0) { Write-Error "[ERROR] 'git fetch' failed."; exit 1 }

Write-Host "[INFO] Switching to '$Main' and updating..."
$current = (git rev-parse --abbrev-ref HEAD).Trim()
if ($current -ne $Main) {
  git checkout -q -- $Main 2>$null
  if ($LASTEXITCODE -ne 0) { Write-Error "[ERROR] Branch '$Main' does not exist or checkout failed."; exit 1 }
} else {
  Write-Host "[INFO] Already on '$Main'."
}

git pull --ff-only $Remote $Main
if ($LASTEXITCODE -ne 0) {
  Write-Warning "[WARN] 'git pull --ff-only' failed (diverged or no upstream). Continuing with local '$Main'."
}

Write-Host "[INFO] Finding remote branches starting with '$Prefix' ..."
# 'git ls-remote --heads' prints: <SHA><TAB>refs/heads/<name>
$ls = git ls-remote --heads $Remote "$Prefix*"

$branches = @()
foreach ($line in $ls) {
  if ([string]::IsNullOrWhiteSpace($line)) { continue }
  $parts = $line -split "`t"
  if ($parts.Length -lt 2) { continue }
  $ref = $parts[1].Trim()            # refs/heads/codex/...
  if ($ref -match '^refs/heads/(.+)$') {
    $branches += $Matches[1]         # codex/...
  }
}

if ($branches.Count -eq 0) {
  Write-Host "[INFO] No remote branches found with prefix '$Prefix' on '$Remote'."
}

foreach ($short in $branches) {
  $remoteBranch = "$Remote/$short"
  Write-Host "------------------------------------------------------------"
  Write-Host "[INFO] Processing $remoteBranch"

  # Make sure we are on MAIN before each merge (quiet if already there)
  $current = (git rev-parse --abbrev-ref HEAD).Trim()
  if ($current -ne $Main) {
    git checkout -q -- $Main 2>$null
    if ($LASTEXITCODE -ne 0) { Write-Error "[ERROR] Could not return to '$Main'."; exit 1 }
  }

  Write-Host "[INFO] Merging $remoteBranch into $Main ..."
  if ($FastForwardOnly) {
    git merge --ff-only $remoteBranch
  } else {
    git merge --no-ff --no-edit $remoteBranch
  }

  if ($LASTEXITCODE -ne 0) {
    Write-Warning "[WARN] Merge failed or has conflicts. Aborting this merge and moving on."
    git merge --abort *> $null
    continue
  }

  Write-Host "[INFO] Pushing '$Main' ..."
  git push $Remote $Main
  if ($LASTEXITCODE -ne 0) { Write-Warning "[WARN] Could not push '$Main'." }

  # Delete local branch if it exists
  git show-ref --verify --quiet "refs/heads/$short"
  if ($LASTEXITCODE -eq 0) {
    Write-Host "[INFO] Deleting local branch '$short' ..."
    git branch -D $short
    if ($LASTEXITCODE -ne 0) { Write-Warning "[WARN] Failed to delete local branch '$short'." }
  } else {
    Write-Host "[INFO] (Local branch '$short' did not exist)"
  }

  # Delete remote branch
  Write-Host "[INFO] Deleting remote branch '$short' on '$Remote' ..."
  git push $Remote --delete $short
  if ($LASTEXITCODE -ne 0) { Write-Warning "[WARN] Could not delete remote branch '$short' (protected?)." }
}

Write-Host ""
Write-Host "[INFO] Switching back to '$startBranch' ..."
git checkout -q -- $startBranch 2>$null

Write-Host "[OK] All done."
